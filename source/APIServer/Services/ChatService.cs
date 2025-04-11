using APIServer.DTOs;
using APIServer.Models;
using APIServer.Utils;
using ServiceStack;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Cryptography;

namespace APIServer.Services
{
    public class ChatService : Service
    {
        public IDbConnectionFactory DB { get; set; }

        public object Post(NewChat request)
        {
            //Xác thực token
            var session = this.GetSession();
            if (!session.IsAuthenticated)
            {
                return new HttpResult(new { Error = "TokenUnauthorized", Message = "User is not authenticated." })
                {
                    StatusCode = HttpStatusCode.Unauthorized
                };
            }

            int userID = int.Parse(session.UserAuthId);

            using (var db = DB.Open())
            {
                var participant = db.Single<Users>(u => u.UserID == request.ParticipantID);

                //Kiểm tra xem người tham gia tồn tại không
                if (participant == null)
                {
                    return new HttpResult(new
                    { Error = "ParticipantNotFound", Message = "The participant does not exist." })
                    {
                        StatusCode = HttpStatusCode.Unauthorized
                    };
                }

                //Các điều kiện privacy
                //...


                var conversationTypeChat = db.Single<ConversationType>(p => p.ConversationTypeName == "CHAT");
                Conversations newConversation = new Conversations
                {
                    CreatorID = userID,
                    ConversationTypeID = conversationTypeChat.ConversationTypeID,
                };
                db.Save(newConversation);

                var roleUser = db.Single<ConversationRole>(p => p.ConversationRoleName == "USER");
                Participants pA = new Participants
                {
                    ConversationID = newConversation.ConversationID,
                    UserID = userID,
                    ConversationRoleID = roleUser.ConversationRoleID
                };
                db.Save(pA);

                //Tạo Aes Key và IV
                (byte[] aesKey, byte[] iv) = E2EEHelper.GenerateAESKey();

                /////////// Thêm Pa key ////////////
                // Lấy Pa public key
                var paPublicKey = db.Single<Users>(u => u.UserID == userID).PublicKey;
                var rsaPublicKey = E2EEHelper.LoadRSAPublicKey(paPublicKey);
                // Mã hóa Aes key bằng public key vừa lấy
                byte[] encryptedAesKey = rsaPublicKey.Encrypt(aesKey, RSAEncryptionPadding.OaepSHA1);

                AesKeys addPaKey = new AesKeys
                {
                    ConversationID = newConversation.ConversationID,
                    UserID = userID,
                    EncryptedAesKey = encryptedAesKey,
                    IV = iv
                };
                db.Save(addPaKey);
                ///////////////////////

                Participants pB = new Participants
                {
                    ConversationID = newConversation.ConversationID,
                    UserID = participant.UserID,
                    ConversationRoleID = roleUser.ConversationRoleID
                };
                db.Save(pB);

                ///////// Thêm Pb key /////////////
                // Lấy Pb public key
                var pbPublicKey = db.Single<Users>(u => u.UserID == participant.UserID).PublicKey;
                var pbRsaPublicKey = E2EEHelper.LoadRSAPublicKey(pbPublicKey);
                // Mã hóa Aes key bằng public key vừa lấy
                byte[] encryptedParticipantAesKey = pbRsaPublicKey.Encrypt(aesKey, RSAEncryptionPadding.OaepSHA1);

                AesKeys addPbKey = new AesKeys
                {
                    ConversationID = newConversation.ConversationID,
                    UserID = participant.UserID,
                    EncryptedAesKey = encryptedParticipantAesKey,
                    IV = iv
                };
                db.Save(addPbKey);
                ///////////////////////

                db.LoadReferences(newConversation);
                db.LoadReferences(pA);
                db.LoadReferences(pB);
                db.LoadReferences(addPaKey);
                db.LoadReferences(addPbKey);

                return new HttpResult(new NewChatResponse
                {
                    Conversation = new ConversationResponse
                    {
                        ConversationID = newConversation.ConversationID,
                        ConversationName = newConversation.ConversationName,
                        ConversationTitle = newConversation.ConversationTitle,
                        CreatorID = newConversation.CreatorID,
                        ConversationType = newConversation.ConversationType.ConversationTypeName,
                        GroupType = newConversation.GroupType?.GroupTypeName ?? null,
                        Participants = new ParticipantResponse[]
                    {
                        new ParticipantResponse
                        {
                            ParticipantID = pA.ParticipantID,
                            ConversationID = newConversation.ConversationID,
                            UserID = pA.UserID,
                            NickName = pA.NickName,
                            ConversationRole = pA.ConversationRole.ConversationRoleName,
                            CreatedAt = pA.CreatedAt
                        },
                        new ParticipantResponse
                        {
                            ParticipantID = pB.ParticipantID,
                            ConversationID = newConversation.ConversationID,
                            UserID = pB.UserID,
                            NickName = pB.NickName,
                            ConversationRole = pB.ConversationRole.ConversationRoleName,
                            CreatedAt = pB.CreatedAt
                        }
                    }
                    }
                }, HttpStatusCode.OK);
            }
        }

        public object Post(DTOs.NewGroup request)
        {
            //Xác thực token
            var session = this.GetSession();
            if (!session.IsAuthenticated)
            {
                return new HttpResult(new { Error = "TokenUnauthorized", Message = "User is not authenticated." })
                {
                    StatusCode = HttpStatusCode.Unauthorized
                };
            }

            int userID = int.Parse(session.UserAuthId);

            using (var db = DB.Open())
            {
                //var participant = db.Single<Users>(u => u.UserID == request.ParticipantID);

                ////Kiểm tra xem người tham gia tồn tại không
                //if (participant == null)
                //{
                //    return new HttpResult(new
                //    { Error = "ParticipantNotFound", Message = "The participant does not exist." })
                //    {
                //        StatusCode = HttpStatusCode.Unauthorized
                //    };
                //}

                //Các điều kiện privacy
                //...

                var conversationTypeChat = db.Single<ConversationType>(p => p.ConversationTypeName == "GROUP");
                var groupTypePrivate = db.Single<GroupType>(p => p.GroupTypeName == "PRIVATE");

                string avatarPath = Utils.Hash.GetMD5Hash(request.GroupAvatar.Concat(BitConverter.GetBytes(DateTime.Now.Ticks)).ToArray());

                VirtualFiles.WriteFile(avatarPath + ".png", request.GroupAvatar);

                Conversations newConversation = new Conversations
                {
                    ConversationName = avatarPath,
                    ConversationTitle = request.GroupTitle,
                    CreatorID = userID,
                    ConversationTypeID = conversationTypeChat.ConversationTypeID,
                    GroupTypeID = groupTypePrivate.GroupTypeID,
                };
                db.Save(newConversation);

                var roleOwner = db.Single<ConversationRole>(p => p.ConversationRoleName == "OWNER");
                var roleUser = db.Single<ConversationRole>(p => p.ConversationRoleName == "USER");

                var newParticipants = new List<Participants>();

                Participants ownerP = new Participants
                {
                    ConversationID = newConversation.ConversationID,
                    UserID = userID,
                    ConversationRoleID = roleOwner.ConversationRoleID
                };
                db.Save(ownerP);
                db.LoadReferences(ownerP);
                newParticipants.Add(ownerP);

                foreach (var participantID in request.Participants)
                {
                    Participants p = new Participants
                    {
                        ConversationID = newConversation.ConversationID,
                        UserID = participantID,
                        ConversationRoleID = roleUser.ConversationRoleID
                    };
                    db.Save(p);
                    db.LoadReferences(p);
                    newParticipants.Add(p);
                }

                var participantResponses = new List<ParticipantResponse>();
                foreach (var participant in newParticipants)
                {
                    participantResponses.Add(new ParticipantResponse
                    {
                        ParticipantID = participant.ParticipantID,
                        ConversationID = newConversation.ConversationID,
                        UserID = participant.UserID,
                        NickName = participant.NickName,
                        ConversationRole = participant.ConversationRole.ConversationRoleName,
                        CreatedAt = participant.CreatedAt
                    });
                }

                db.LoadReferences(newConversation);
                return new HttpResult(new DTOs.NewChatResponse
                {
                    Conversation = new ConversationResponse
                    {
                        ConversationID = newConversation.ConversationID,
                        ConversationName = newConversation.ConversationName,
                        ConversationTitle = newConversation.ConversationTitle,
                        CreatorID = newConversation.CreatorID,
                        ConversationType = newConversation.ConversationType.ConversationTypeName,
                        GroupType = newConversation.GroupType?.GroupTypeName ?? null,
                        Participants = participantResponses.ToArray()
                    }
                }, HttpStatusCode.OK);
            }
        }

        public object Post(DTOs.DeleteConversation request)
        {
            //Xác thực token
            var session = this.GetSession();
            if (!session.IsAuthenticated)
            {
                return new HttpResult(new { Error = "TokenUnauthorized", Message = "User is not authenticated." })
                {
                    StatusCode = HttpStatusCode.Unauthorized
                };
            }

            int userID = int.Parse(session.UserAuthId);

            using (var db = DB.Open())
            {
                //Kiểm tra có cuộc trò chuyện không
                var conversation = db.Single<Conversations>(p => p.ConversationID == request.ConversationID);
                if (conversation == null)
                {
                    return new HttpResult(new
                    { Error = "ConversationNotFound", Message = "Conversation does not exist." })
                    {
                        StatusCode = HttpStatusCode.OK
                    };
                }

                //Kiểm tra có tham gia cuộc trò chuyện không
                var participant = db.Single<Participants>(p =>
                    p.UserID == userID && p.ConversationID == conversation.ConversationID);
                if (participant == null)
                {
                    return new HttpResult(new
                    { Error = "ParticipantNotFound", Message = "You are not a participant in the conversation." })
                    {
                        StatusCode = HttpStatusCode.OK
                    };
                }

                db.LoadReferences(conversation);
                db.LoadReferences(participant);

                //Kiểm tra có phải là chủ không đối với Group
                if (conversation.ConversationType.ConversationTypeName == "GROUP" &&
                    participant.ConversationRole.ConversationRoleName != "OWNER")
                {
                    return new HttpResult(new
                    { Error = "PermissionDenied", Message = "You do not have permission to delete." })
                    {
                        StatusCode = HttpStatusCode.OK
                    };
                }

                var deleteConversation = new Models.DeleteConversations
                {
                    ConversationID = conversation.ConversationID,
                    UserID = userID,
                };
                db.Save(deleteConversation);

                return new HttpResult(new DeleteConversationResponse { }, HttpStatusCode.OK);
            }
        }

        public object Get(DTOs.GetConversations request)
        {
            var session = this.GetSession();
            if (!session.IsAuthenticated)
            {
                return new HttpResult(new { Error = "TokenUnauthorized", Message = "User is not authenticated." })
                {
                    StatusCode = HttpStatusCode.Unauthorized
                };
            }

            int userID = int.Parse(session.UserAuthId);

            using (var db = DB.Open())
            {
                List<ConversationResponse> conversationResponses = new List<ConversationResponse>();

                var conversationIds = db.Column<int>(
                    db.From<Participants>().Where(p => p.UserID == userID).Select(p => p.ConversationID)
                );

                var conversations = db.Select<Conversations>()
                    .Where(c => conversationIds.Contains(c.ConversationID) && !db.Exists(db.From<DeleteConversations>().Where(dc => dc.ConversationID == c.ConversationID)));

                foreach (var conversation in conversations)
                {
                    db.LoadReferences(conversation);

                    if (conversation.ConversationType.ConversationTypeName == "CHAT")
                    {
                        var isBlock = db.Exists(db.From<Participants>()
                            .Join<Contacts>((p, c) => c.ContactID == userID && c.UserID == p.UserID && c.BlockAt != null)
                            .Where(p => p.ConversationID == conversation.ConversationID));

                        if (isBlock)
                        {
                            continue;
                        }
                    }

                    var participants = db.Select(db.From<Participants>()
                        .Where(c => c.ConversationID == conversation.ConversationID));

                    List<ParticipantResponse> participantResponses = new List<ParticipantResponse>();
                    foreach (var participant in participants)
                    {
                        db.LoadReferences(participant);
                        participantResponses.Add(new ParticipantResponse
                        {
                            ParticipantID = participant.ParticipantID,
                            ConversationID = participant.ConversationID,
                            UserID = participant.UserID,
                            NickName = participant.NickName,
                            ConversationRole = participant.ConversationRole.ConversationRoleName,
                            CreatedAt = participant.CreatedAt
                        });
                    }

                    var latestMessage = db.Single<Models.Messages>(
                        db.From<Models.Messages>()
                            .Where(m => m.ConversationID == conversation.ConversationID)
                            .OrderByDescending(m => m.CreatedAt)
                            .Limit(1)
                    );

                    string latestMessageText = string.Empty;
                    DateTime latestMessageTime = conversation.CreatedAt;
                    if (latestMessage != null)
                    {
                        db.LoadReferences(latestMessage);
                        latestMessageTime = latestMessage.CreatedAt;
                        if (latestMessage.MessageTypeRef.MessageTypeName == "TEXT")
                        {
                            if (latestMessage.Content == "")
                            {
                                long attachmentCount =
                                    db.Count<Attachments>(a => a.MessageID == latestMessage.MessageID);
                                latestMessageText = latestMessage.SenderID == userID
                                    ? $"Bạn đã gửi {attachmentCount.ToString()} tệp đính kèm"
                                    : $"Đã nhận {attachmentCount.ToString()} tệp đính kèm";
                            }
                            else
                            {
                                // Lấy AES key và IV của cuộc trò chuyện
                                var aesKey = db.Single<AesKeys>(k => k.ConversationID == conversation.ConversationID && k.UserID == userID);

                                // Lấy username
                                var user = db.Single<Users>(u => u.UserID == userID);
                                if (user == null)
                                {
                                    return new HttpResult(new { Error = "UserNotFound", Message = "User not found." })
                                    {
                                        StatusCode = HttpStatusCode.NotFound
                                    };
                                }

                                //// Lấy PrivateKey từ registry
                                var userPrivateKeyFromRegistry = E2EEHelper.LoadFromRegistry(user.Username);
                                RSA rsaPrivateKey = E2EEHelper.LoadRSAPrivateKey(userPrivateKeyFromRegistry);
                                byte[] decryptedAesKey = new byte[0];
                                string decryptedContent = "";
                                if (aesKey.EncryptedAesKey != null)
                                {
                                    // Giải mã Aes key bằng private key vừa lấy
                                    decryptedAesKey = rsaPrivateKey.Decrypt(aesKey.EncryptedAesKey, RSAEncryptionPadding.OaepSHA1);

                                    // Giải mã tin nhắn bằng AES key vừa giải mã
                                    decryptedContent = E2EEHelper.DecryptMessage(latestMessage.Content, decryptedAesKey, aesKey.IV);
                                }
                                else
                                {
                                    decryptedContent = "Tin nhắn đã được mã hóa.";
                                }
                                latestMessageText = latestMessage.SenderID == userID
                                    ? $"Bạn: {decryptedContent}"
                                    : decryptedContent;
                            }
                        }
                        else if (latestMessage.MessageTypeRef.MessageTypeName == "CALL")
                        {
                            latestMessageText = latestMessage.SenderID == userID ? "Bạn đã gọi" : "Đã nhận cuộc gọi";
                        }
                        else if (latestMessage.MessageTypeRef.MessageTypeName == "AUDIO")
                        {
                            latestMessageText =
                                latestMessage.SenderID == userID ? "Bạn đã gửi" : "Đã nhận một âm thanh";
                        }
                    }

                    ConversationResponse conversationResponse = new ConversationResponse
                    {
                        ConversationID = conversation.ConversationID,
                        ConversationName = conversation.ConversationName,
                        ConversationTitle = conversation.ConversationTitle,
                        CreatorID = conversation.CreatorID,
                        ConversationType = conversation.ConversationType.ConversationTypeName,
                        GroupType = conversation.GroupType?.GroupTypeName ?? null,
                        CreatedAt = conversation.CreatedAt,

                        Participants = participantResponses.ToArray(),
                        LatestMessage = latestMessageText,
                        LatestMessageTime = latestMessageTime
                    };
                    conversationResponses.Add(conversationResponse);
                }

                return new HttpResult(new GetConversationResponse() { Conversations = conversationResponses.ToArray() },
                    HttpStatusCode.OK);
            }
        }

        public object Get(DTOs.GetMessages request)
        {
            var session = this.GetSession();
            if (!session.IsAuthenticated)
            {
                return new HttpResult(new { Error = "TokenUnauthorized", Message = "User is not authenticated." })
                {
                    StatusCode = HttpStatusCode.Unauthorized
                };
            }

            int userID = int.Parse(session.UserAuthId);

            using (var db = DB.Open())
            {
                Participants participant =
                    db.Single<Participants>(p => p.ConversationID == request.ConversationID && p.UserID == userID);
                if (participant == null)
                {
                    return new HttpResult(new
                    { Error = "ParticipantNotFound", Message = "You are not a participant in the conversation." })
                    {
                        StatusCode = HttpStatusCode.OK
                    };
                }

                var typeOnlyMe = db.Single<DeleteType>(dt => dt.DeleteTypeName == "ONLYME");
                var typeAll = db.Single<DeleteType>(dt => dt.DeleteTypeName == "ALL");

                var query = db.From<Models.Messages>().Where(x => x.ConversationID == request.ConversationID)
                    .LeftJoin<MessageDelete>((m, md) => m.MessageID == md.MessageID &&
                                                        ((md.DeleteTypeID == typeAll.DeleteTypeID) ||
                                                         (md.DeleteTypeID == typeOnlyMe.DeleteTypeID &&
                                                          m.SenderID == userID)));

                if (request.Before != null)
                    query.Where(x => x.CreatedAt < request.Before);

                if (request.After != null)
                    query.Where(x => x.CreatedAt > request.After);

                query.OrderBy(x => x.CreatedAt);

                if (request.Limit != null)
                    query.Limit(request.Limit.Value);

                List<MessageResponse> messageResponses = new List<MessageResponse>();
                foreach (var message in db.Select<Models.Messages>(query))
                {
                    var attachments = db.Select(db.From<Attachments>()
                        .Where(a => a.MessageID == message.MessageID));

                    List<AttachmentResponse> attachmentResponses = new List<AttachmentResponse>();
                    if (attachments != null)
                    {
                        foreach (var attachment in attachments)
                        {
                            db.LoadReferences(attachment);

                            attachmentResponses.Add(new AttachmentResponse
                            {
                                AttachmentID = attachment.AttachmentID,
                                AttachmentType = attachment.AttachmentType.AttachmentTypeName,
                                ThumnailURL = attachment.ThumbnailURL,
                                FileURL = attachment.FileURL,
                                CreatedAt = attachment.CreatedAt,
                            });
                        }
                    }
                    else
                    {
                        attachmentResponses = null;
                    }

                    db.LoadReferences(message);

                    // Lấy AES key và IV của cuộc trò chuyện
                    var aesKey = db.Single<AesKeys>(k => k.ConversationID == request.ConversationID && k.UserID == userID);

                    // Lấy username
                    var user = db.Single<Users>(u => u.UserID == userID);
                    if (user == null)
                    {
                        return new HttpResult(new { Error = "UserNotFound", Message = "User not found." })
                        {
                            StatusCode = HttpStatusCode.NotFound
                        };
                    }

                    //// Lấy PrivateKey từ registry
                    var userPrivateKeyFromRegistry = E2EEHelper.LoadFromRegistry(user.Username);
                    RSA rsaPrivateKey = E2EEHelper.LoadRSAPrivateKey(userPrivateKeyFromRegistry);
                    byte[] decryptedAesKey = new byte[0];
                    string decryptedContent = "";
                    if (aesKey.EncryptedAesKey != null && rsaPrivateKey != null)
                    {
                        // Giải mã Aes key bằng private key vừa lấy
                        decryptedAesKey = rsaPrivateKey.Decrypt(aesKey.EncryptedAesKey, RSAEncryptionPadding.OaepSHA1);

                        // Giải mã tin nhắn bằng AES key vừa giải mã
                        decryptedContent = E2EEHelper.DecryptMessage(message.Content, decryptedAesKey, aesKey.IV);
                    }
                    else
                    {
                        decryptedContent = "Tin nhắn đã được mã hóa.";
                    }

                    messageResponses.Add(new MessageResponse
                    {
                        MessageID = message.MessageID,
                        Sender = new UserResponse
                        {
                            UserID = message.Sender.UserID,
                            Username = message.Sender.Username,
                            FirstName = message.Sender.FirstName,
                            LastName = message.Sender.LastName,
                            Avatar = message.Sender.Avatar,
                        },
                        Content = decryptedContent,
                        MessageType = message.MessageTypeRef.MessageTypeName,
                        CreatedAt = message.CreatedAt,
                        Attachments = attachmentResponses?.ToArray()
                    });
                }

                return new HttpResult(new GetMessagesResponse() { Messages = messageResponses.ToArray() },
                    HttpStatusCode.OK);
            }
        }

        public object Post(DTOs.DeleteMessage request)
        {
            var session = this.GetSession();
            if (!session.IsAuthenticated)
            {
                return new HttpResult(new { Error = "TokenUnauthorized", Message = "User is not authenticated." })
                {
                    StatusCode = HttpStatusCode.Unauthorized
                };
            }

            int userID = int.Parse(session.UserAuthId);

            using (var db = DB.Open())
            {
                var message = db.Single<Models.Messages>(m => m.MessageID == request.MessageID);
                if (message == null)
                {
                    return new HttpResult(new
                    { Error = "MessageNotFound", Message = "Message doenst not exist." })
                    {
                        StatusCode = HttpStatusCode.OK
                    };
                }

                Participants participant =
                    db.Single<Participants>(p => p.ConversationID == message.ConversationID && p.UserID == userID);
                if (participant == null)
                {
                    return new HttpResult(new
                    { Error = "ParticipantNotFound", Message = "You are not a participant in the conversation." })
                    {
                        StatusCode = HttpStatusCode.OK
                    };
                }

                DeleteType deleteType = db.Single<DeleteType>(dt => dt.DeleteTypeName == request.DeleteType);
                if (deleteType == null)
                {
                    return new HttpResult(new
                    { Error = "DeletyeTypeNotFound", Message = "Delete type doenst not exist." })
                    {
                        StatusCode = HttpStatusCode.OK
                    };
                }

                MessageDelete messageDelete = new MessageDelete
                {
                    MessageID = message.MessageID,
                    DeleteByUserID = userID,
                    DeleteTypeID = deleteType.DeleteTypeID,
                };

                db.Insert(messageDelete);

                return new HttpResult(new DeleteMessageResponse { }, HttpStatusCode.OK);
            }
        }

        public object Post(DTOs.ChangeNickname request)
        {
            var session = this.GetSession();
            if (!session.IsAuthenticated)
            {
                return new HttpResult(new { Error = "TokenUnauthorized", Message = "User is not authenticated." })
                {
                    StatusCode = HttpStatusCode.Unauthorized
                };
            }

            int userID = int.Parse(session.UserAuthId);

            using (var db = DB.Open())
            {
                Participants participant =
                    db.Single<Participants>(p => p.ConversationID == request.ConversationID && p.UserID == userID);
                if (participant == null)
                {
                    return new HttpResult(new
                    { Error = "ParticipantNotFound", Message = "You are not a participant in the conversation." })
                    {
                        StatusCode = HttpStatusCode.OK
                    };
                }

                participant.NickName = request.Nickname;
                db.Update(participant);

                return new HttpResult(new DTOs.ChangeNicknameResponse { }, HttpStatusCode.OK);
            }
        }

        public object Get(DTOs.FindChat request)
        {
            var session = this.GetSession();
            if (!session.IsAuthenticated)
            {
                return new HttpResult(new { Error = "TokenUnauthorized", Message = "User is not authenticated." })
                {
                    StatusCode = HttpStatusCode.Unauthorized
                };
            }

            int userID = int.Parse(session.UserAuthId);

            using (var db = DB.Open())
            {
                var conversationTypeChat = db.Single<ConversationType>(p => p.ConversationTypeName == "CHAT");

                var conversation = db.Single<Conversations>(@"
    SELECT * FROM Conversations c
    WHERE c.ConversationTypeID = @conversationTypeID
    AND EXISTS (
        SELECT 1 FROM Participants p
        WHERE p.ConversationID = c.ConversationID AND p.UserID = @userID
    )
    AND EXISTS (
        SELECT 1 FROM Participants p
        WHERE p.ConversationID = c.ConversationID AND p.UserID = @requestUserID
    )",
    new { conversationTypeID = conversationTypeChat.ConversationTypeID, userID, requestUserID = request.UserID }
);

                if (conversation == null || db.Exists(db.From<DeleteConversations>().Where(dc => dc.ConversationID == conversation.ConversationID)))
                {
                    return new HttpResult(new FindChatResponse() { Conversation = null },
                        HttpStatusCode.OK);
                }

                var participants = db.Select(db.From<Participants>()
                    .Where(c => c.ConversationID == conversation.ConversationID));

                List<ParticipantResponse> participantResponses = new List<ParticipantResponse>();
                foreach (var participant in participants)
                {
                    db.LoadReferences(participant);
                    participantResponses.Add(new ParticipantResponse
                    {
                        ParticipantID = participant.ParticipantID,
                        ConversationID = participant.ConversationID,
                        UserID = participant.UserID,
                        NickName = participant.NickName,
                        ConversationRole = participant.ConversationRole.ConversationRoleName,
                        CreatedAt = participant.CreatedAt
                    });
                }

                var latestMessage = db.Single<Models.Messages>(
                    db.From<Models.Messages>()
                        .Where(m => m.ConversationID == conversation.ConversationID)
                        .OrderByDescending(m => m.CreatedAt)
                        .Limit(1)
                );

                string latestMessageText = "";
                if (latestMessage != null)
                {
                    db.LoadReferences(latestMessage);
                    if (latestMessage.MessageTypeRef.MessageTypeName == "TEXT")
                    {
                        if (latestMessage.Content == "")
                        {
                            long attachmentCount =
                                db.Count<Attachments>(a => a.MessageID == latestMessage.MessageID);
                            latestMessageText = latestMessage.SenderID == userID
                                ? $"Bạn đã gửi {attachmentCount.ToString()} tệp đính kèm"
                                : $"Đã nhận {attachmentCount.ToString()} tệp đính kèm";
                        }
                        else
                        {
                            latestMessageText = latestMessage.SenderID == userID
                                ? $"Bạn: {latestMessage.Content}"
                                : latestMessage.Content;
                        }
                    }
                    else if (latestMessage.MessageTypeRef.MessageTypeName == "CALL")
                    {
                        latestMessageText = latestMessage.SenderID == userID ? "Bạn đã gọi" : "Đã nhận cuộc gọi";
                    }
                    else if (latestMessage.MessageTypeRef.MessageTypeName == "AUDIO")
                    {
                        latestMessageText =
                            latestMessage.SenderID == userID ? "Bạn đã gửi" : "Đã nhận một âm thanh";
                    }
                }
                db.LoadReferences(conversation);
                ConversationResponse conversationResponse = new ConversationResponse
                {
                    ConversationID = conversation.ConversationID,
                    ConversationName = conversation.ConversationName,
                    ConversationTitle = conversation.ConversationTitle,
                    CreatorID = conversation.CreatorID,
                    ConversationType = conversation.ConversationType.ConversationTypeName,
                    GroupType = conversation.GroupType?.GroupTypeName ?? null,
                    CreatedAt = conversation.CreatedAt,

                    Participants = participantResponses.ToArray(),
                    LatestMessage = latestMessageText
                };

                return new HttpResult(new FindChatResponse() { Conversation = conversationResponse },
                    HttpStatusCode.OK);
            }
        }

        public object Get(DTOs.FindConversation request)
        {
            var session = this.GetSession();
            if (!session.IsAuthenticated)
            {
                return new HttpResult(new { Error = "TokenUnauthorized", Message = "User is not authenticated." })
                {
                    StatusCode = HttpStatusCode.Unauthorized
                };
            }

            int userID = int.Parse(session.UserAuthId);

            using (var db = DB.Open())
            {
                var conversation = db.Single<Conversations>(
                    db.From<Conversations>()
                    .Where(c => c.ConversationID == request.ConversationID && !db.Exists(db.From<DeleteConversations>().Where(dc => dc.ConversationID == c.ConversationID)))
                    );

                if (conversation == null)
                {
                    return new HttpResult(new FindChatResponse() { Conversation = null },
                        HttpStatusCode.OK);
                }

                var participants = db.Select(db.From<Participants>()
                    .Where(c => c.ConversationID == conversation.ConversationID));

                List<ParticipantResponse> participantResponses = new List<ParticipantResponse>();
                foreach (var participant in participants)
                {
                    db.LoadReferences(participant);
                    participantResponses.Add(new ParticipantResponse
                    {
                        ParticipantID = participant.ParticipantID,
                        ConversationID = participant.ConversationID,
                        UserID = participant.UserID,
                        NickName = participant.NickName,
                        ConversationRole = participant.ConversationRole.ConversationRoleName,
                        CreatedAt = participant.CreatedAt
                    });
                }

                var latestMessage = db.Single<Models.Messages>(
                    db.From<Models.Messages>()
                        .Where(m => m.ConversationID == conversation.ConversationID)
                        .OrderBy(m => m.CreatedAt)
                );

                string latestMessageText = "";
                if (latestMessage != null)
                {
                    db.LoadReferences(latestMessage);
                    if (latestMessage.MessageTypeRef.MessageTypeName == "TEXT")
                    {
                        if (latestMessage.Content == "")
                        {
                            long attachmentCount =
                                db.Count<Attachments>(a => a.MessageID == latestMessage.MessageID);
                            latestMessageText = latestMessage.SenderID == userID
                                ? $"Bạn đã gửi {attachmentCount.ToString()} tệp đính kèm"
                                : $"Đã nhận {attachmentCount.ToString()} tệp đính kèm";
                        }
                        else
                        {
                            latestMessageText = latestMessage.SenderID == userID
                                ? $"Bạn: {latestMessage.Content}"
                                : latestMessage.Content;
                        }
                    }
                    else if (latestMessage.MessageTypeRef.MessageTypeName == "CALL")
                    {
                        latestMessageText = latestMessage.SenderID == userID ? "Bạn đã gọi" : "Đã nhận cuộc gọi";
                    }
                    else if (latestMessage.MessageTypeRef.MessageTypeName == "AUDIO")
                    {
                        latestMessageText =
                            latestMessage.SenderID == userID ? "Bạn đã gửi" : "Đã nhận một âm thanh";
                    }
                }

                db.LoadReferences(conversation);
                ConversationResponse conversationResponse = new ConversationResponse
                {
                    ConversationID = conversation.ConversationID,
                    ConversationName = conversation.ConversationName,
                    ConversationTitle = conversation.ConversationTitle,
                    CreatorID = conversation.CreatorID,
                    ConversationType = conversation.ConversationType.ConversationTypeName,
                    GroupType = conversation.GroupType?.GroupTypeName ?? null,
                    CreatedAt = conversation.CreatedAt,

                    Participants = participantResponses.ToArray(),
                    LatestMessage = latestMessageText
                };

                return new HttpResult(new FindChatResponse() { Conversation = conversationResponse },
                    HttpStatusCode.OK);
            }
        }

        public object Post(DTOs.SendMessage request)
        {
            var session = this.GetSession();
            if (!session.IsAuthenticated)
            {
                return new HttpResult(new { Error = "TokenUnauthorized", Message = "User is not authenticated." })
                {
                    StatusCode = HttpStatusCode.Unauthorized
                };
            }

            int userID = int.Parse(session.UserAuthId);

            using (var db = DB.Open())
            {
                Participants participant =
                  db.Single<Participants>(p => p.ConversationID == request.ConversationID && p.UserID == userID);
                if (participant == null)
                {
                    return new HttpResult(new
                    { Error = "ParticipantNotFound", Message = "You are not a participant in the conversation." })
                    {
                        StatusCode = HttpStatusCode.Unauthorized
                    };
                }
                db.LoadReferences(participant);

                MessageType messageType = db.Single<MessageType>(dt => dt.MessageTypeName == request.MessageType);
                if (messageType == null)
                {
                    return new HttpResult(new
                    { Error = "MessageTypeNotFound", Message = "Delete type doenst not exist." })
                    {
                        StatusCode = HttpStatusCode.NotFound
                    };
                }

                Models.Messages newMessage = new Models.Messages
                {
                    ConversationID = request.ConversationID,
                    SenderID = userID,
                    Content = request.Content,
                    MessageType = messageType.MessageTypeID,
                };

                // Get AES key and IV for this conversation
                var aesKey = db.Single<AesKeys>(k => k.ConversationID == request.ConversationID && k.UserID == userID);
                if (aesKey == null)
                {
                    return new HttpResult(new { Error = "AesKeyNotFound", Message = "AES key not found for this conversation." })
                    {
                        StatusCode = HttpStatusCode.NotFound
                    };
                }

                // Get username from Users table
                var user = db.Single<Users>(u => u.UserID == userID);
                if (user == null)
                {
                    return new HttpResult(new { Error = "UserNotFound", Message = "User not found." })
                    {
                        StatusCode = HttpStatusCode.NotFound
                    };
                }

                //// Lấy private key của user từ registry
                var userPrivateKeyFromRegistry = E2EEHelper.LoadFromRegistry(user.Username);
                RSA rsaPrivateKey = E2EEHelper.LoadRSAPrivateKey(userPrivateKeyFromRegistry);
                if (rsaPrivateKey != null)
                {
                    //// Giải mã AES key đã bị mã hóa bằng Private Key đã lấy trước đó
                    byte[] decryptedAesKey = rsaPrivateKey.Decrypt(aesKey.EncryptedAesKey, RSAEncryptionPadding.OaepSHA1);
                    //// Mã hóa tin nhắn bằng AES
                    string encryptedContent = E2EEHelper.EncryptMessage(request.Content, decryptedAesKey, aesKey.IV);
                    newMessage.Content = encryptedContent;
                }
                else
                {
                    return new HttpResult(new { Error = "PrivateKeyNotFound", Message = "RSA private key could not be loaded." })
                    {
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }
                db.Save(newMessage);

                Debug.WriteLine(newMessage.Content);

                List<AttachmentResponse> attachments = new List<AttachmentResponse>();
                if (request.Attachments != null)
                {
                    List<Attachments> attachmentInserts = new List<Attachments>();
                    foreach (var attachment in request.Attachments)
                    {
                        AttachmentType attachmentType = db.Single<AttachmentType>(a => a.AttachmentTypeName == attachment.AttachmentType);
                        if (attachmentType == null)
                        {
                            return new HttpResult(new
                            { Error = "AttachmentTypeNotFound", Message = "Attachment type doenst not exist." })
                            {
                                StatusCode = HttpStatusCode.OK
                            };
                        }

                        string fileName = string.Format("{0}_{1}", Hash.GetMD5HashByString(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + Hash.GetMD5Hash(attachment.FileData)).Substring(0, 8), attachment.FileName.Replace(" ", "_"));

                        VirtualFiles.WriteFile(fileName, attachment.FileData);

                        attachmentInserts.Add(new Attachments
                        {
                            MessageID = newMessage.MessageID,
                            AttachmentTypeID = attachmentType.AttachmentTypeID,
                            ThumbnailURL = "",
                            FileURL = fileName
                        });
                    }

                    foreach (var attachmentInsert in attachmentInserts)
                    {
                        db.Save(attachmentInsert);
                        db.LoadReferences(attachmentInsert);
                        attachments.Add(new AttachmentResponse
                        {
                            AttachmentID = attachmentInsert.AttachmentID,
                            AttachmentType = attachmentInsert.AttachmentType.AttachmentTypeName,
                            ThumnailURL = attachmentInsert.ThumbnailURL,
                            FileURL = attachmentInsert.FileURL,
                            CreatedAt = attachmentInsert.CreatedAt
                        });
                    }
                }

                MessageResponse messageResponse = new MessageResponse
                {
                    MessageID = newMessage.MessageID,
                    Sender = new UserResponse
                    {
                        UserID = participant.User.UserID,
                        Username = participant.User.Username,
                        FirstName = participant.User.FirstName,
                        LastName = participant.User.LastName,
                        Avatar = participant.User.Avatar,
                    },
                    Content = newMessage.Content,
                    MessageType = request.MessageType,
                    CreatedAt = newMessage.CreatedAt,
                    Attachments = attachments.ToArray()
                };

                return new HttpResult(new SendMessageResponse
                {
                    Message = messageResponse,
                }, HttpStatusCode.OK);
            }
        }
    }
}
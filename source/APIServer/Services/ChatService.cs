using APIServer.DTOs;
using APIServer.Models;
using APIServer.Utils;
using ServiceStack;
using ServiceStack.Data;
using ServiceStack.IO;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

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
                var participant = db.Single<User>(u => u.UserID == request.ParticipantID);

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
                Conversation newConversation = new Conversation
                {
                    CreatorID = userID,
                    ConversationTypeID = conversationTypeChat.ConversationTypeID,
                };
                db.Save(newConversation);

                var roleUser = db.Single<ConversationRole>(p => p.ConversationRoleName == "USER");
                Participant pA = new Participant
                {
                    ConversationID = newConversation.ConversationID,
                    UserID = userID,
                    ConversationRoleID = roleUser.ConversationRoleID
                };
                db.Save(pA);

                Participant pB = new Participant
                {
                    ConversationID = newConversation.ConversationID,
                    UserID = participant.UserID,
                    ConversationRoleID = roleUser.ConversationRoleID
                };
                db.Save(pB);

                db.LoadReferences(newConversation);
                db.LoadReferences(pA);
                db.LoadReferences(pB);
                return new HttpResult(new DTOs.NewChatResponse
                {
                    ConversationID = newConversation.ConversationID,
                    ConversationName = newConversation.ConversationName,
                    ConversationTitle = newConversation.ConversationTitle,
                    CreatorID = newConversation.CreatorID,
                    ConversationType = newConversation.ConversationType.ConversationTypeName,
                    GroupType = newConversation.GroupType?.GroupTypeName ?? null,
                    CreateAt = newConversation.CreatedAt,
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
                var conversation = db.Single<Conversation>(p => p.ConversationID == request.ConversationID);
                if (conversation == null)
                {
                    return new HttpResult(new
                    { Error = "ConversationNotFound", Message = "Conversation does not exist." })
                    {
                        StatusCode = HttpStatusCode.OK
                    };
                }

                //Kiểm tra có tham gia cuộc trò chuyện không
                var participant = db.Single<Participant>(p =>
                    p.UserID == userID && p.ConversationID == conversation.ConversationID);
                if (participant == null)
                {
                    return new HttpResult(new
                    { Error = "ParticipantNotFound", Message = "You are not a participant in the conversation." })
                    {
                        StatusCode = HttpStatusCode.OK
                    };
                }

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

                var deleteConversation = new Models.DeleteConversation
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
                    db.From<Participant>().Where(p => p.UserID == userID).Select(p => p.ConversationID)
                );

                var conversations = db.Select<Conversation>(
                    db.From<Conversation>().Where(c => conversationIds.Contains(c.ConversationID))
                );

                foreach (var conversation in conversations)
                {
                    var participants = db.Select(db.From<Participant>()
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


                    var latestMessage = db.Single<Message>(
                        db.From<Message>()
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
                                    db.Count<Attachment>(a => a.MessageID == latestMessage.MessageID);
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
                Participant participant =
                    db.Single<Participant>(p => p.ConversationID == request.ConversationID && p.UserID == userID);
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

                var query = db.From<Message>().Where(x => x.ConversationID == request.ConversationID)
                    .LeftJoin<MessageDelete>((m, md) => m.MessageID == md.MessageID &&
                                                        ((md.DeleteTypeID == typeAll.DeleteTypeID) ||
                                                         (md.DeleteTypeID == typeOnlyMe.DeleteTypeID &&
                                                          m.SenderID == userID)));

                if (request.Before != null)
                    query.Where(x => x.CreatedAt < request.Before);

                if (request.After != null)
                    query.Where(x => x.CreatedAt > request.After);

                query.OrderByDescending(x => x.CreatedAt);

                if (request.Limit != null)
                    query.Limit(request.Limit.Value);

                List<MessageResponse> messageResponses = new List<MessageResponse>();
                foreach (var message in db.Select<Message>(query))
                {
                    var attachments = db.Select(db.From<Attachment>()
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
                        Content = message.Content,
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
                var message = db.Single<Message>(m => m.MessageID == request.MessageID);
                if (message == null)
                {
                    return new HttpResult(new
                    { Error = "MessageNotFound", Message = "Message doenst not exist." })
                    {
                        StatusCode = HttpStatusCode.OK
                    };
                }

                Participant participant =
                    db.Single<Participant>(p => p.ConversationID == message.ConversationID && p.UserID == userID);
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
                Participant participant =
                    db.Single<Participant>(p => p.ConversationID == request.ConversationID && p.UserID == userID);
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
                Participant participant =
                  db.Single<Participant>(p => p.ConversationID == request.ConversationID && p.UserID == userID);
                if (participant == null)
                {
                    return new HttpResult(new
                    { Error = "ParticipantNotFound", Message = "You are not a participant in the conversation." })
                    {
                        StatusCode = HttpStatusCode.OK
                    };
                }

                MessageType messageType = db.Single<MessageType>(dt => dt.MessageTypeName == request.MessageType);
                if (messageType == null)
                {
                    return new HttpResult(new
                    { Error = "MessageTypeNotFound", Message = "Delete type doenst not exist." })
                    {
                        StatusCode = HttpStatusCode.OK
                    };
                }

                Message newMessage = new Message
                {
                    ConversationID = request.ConversationID,
                    SenderID = userID,
                    Content = request.Content,
                    MessageType = messageType.MessageTypeID,
                };

                db.Save(newMessage);

                List<Attachment> attachmentInserts = new List<Attachment>();
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

                    string fileName = string.Format("{0}_{1}", Hash.GetMD5HashByString(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + Hash.GetMD5Hash(attachment.FileData)).Substring(0,8), attachment.FileName.Replace(" ","_"));

                    VirtualFiles.WriteFile(fileName, attachment.FileData);

                    attachmentInserts.Add(new Attachment
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
                }

               //...

                return new HttpResult(new SendMessageResponse {

                }, HttpStatusCode.OK);
            }
        }
    }
}
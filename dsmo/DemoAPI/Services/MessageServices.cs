using DemoAPI.Models;
using ServiceStack;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using System.Collections.Generic;
using System.Net;

namespace DemoAPI.Services
{
    public class MessageService : Service
    {
        public IDbConnectionFactory DB { get; set; }
        public object Post(DTOs.SendMessage request)
        {
            var session = this.GetSession();
            if (!session.IsAuthenticated)
                throw new HttpError(HttpStatusCode.Unauthorized, "TokenUnauthorized", "please login!!");

            using (var db = DB.Open())
            {
                var newMessage = new Message
                {
                    ConversationsID = request.ConversationID,
                    SenderID = int.Parse(session.UserAuthId),
                    Content = request.Content,
                    MessageType = request.MessageType,
                };

                long messageID = db.Insert(newMessage, selectIdentity: true);

                return new HttpResult(new DTOs.SendMessageResponse
                {
                    Success = true,
                    MessageId = (int)messageID,
                }, HttpStatusCode.OK);
            }
        }

        public object Post(DTOs.NewConversation request)
        {
            var session = this.GetSession();
            if (!session.IsAuthenticated)
                throw new HttpError(HttpStatusCode.Unauthorized, "TokenUnauthorized", "please login!!");

            using (var db = DB.Open())
            {
                var newConversation = new Conversation
                {
                    UserAID = int.Parse(session.UserAuthId),
                    UserBID = request.WithUserID,
                };

                long conversationID = db.Insert(newConversation, selectIdentity: true);

                return new HttpResult(new DTOs.NewConversationResponse
                {
                    Success = true,
                    ConversationID = (int)conversationID,
                }, HttpStatusCode.OK);
            }
        }

        public object Get(DTOs.GetByConversation request)
        {
            var session = this.GetSession();
            if (!session.IsAuthenticated)
                throw new HttpError(HttpStatusCode.Unauthorized, "TokenUnauthorized", "please login!!");

            using (var db = DB.Open())
            {
                var messages = db.From<Message>().Where(x => x.ConversationsID == request.ConversationID);

                if (request.Beforce != null)
                    messages.Where(x => x.CreatedAt < request.Beforce);

                if (request.Beforce != null)
                    messages.Where(x => x.CreatedAt > request.After);

                if (request.Limit != null)
                    messages.OrderByDescending(x => x.CreatedAt)
                      .Limit(request.Limit.GetValueOrDefault());

                return new HttpResult(messages, HttpStatusCode.OK);
            }
        }

        public object Get(DTOs.GetConversations request)
        {
            var session = this.GetSession();
            if (!session.IsAuthenticated)
                throw new HttpError(HttpStatusCode.Unauthorized, "TokenUnauthorized", "please login!!");

            using (var db = DB.Open())
            {
                int userID = int.Parse(session.UserAuthId);
                var conversations = db.Select(db.From<Conversation>().Where(x => x.UserAID == userID || x.UserBID == userID));

                List<DTOs.ConversationItem> conversationList = new List<DTOs.ConversationItem>();
                foreach (var conversation in conversations)
                {
                    conversationList.Add(new DTOs.ConversationItem { ConversationID = conversation.ConversationID, ReceiverID = conversation.UserAID == userID ? conversation.UserBID : conversation.UserAID });
                }
                return new HttpResult(conversationList, HttpStatusCode.OK);
            }
        }
    }
}

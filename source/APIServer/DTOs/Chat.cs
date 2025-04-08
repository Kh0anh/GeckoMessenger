using ServiceStack;
using System;

namespace APIServer.DTOs
{
    [Route("/chat/newChat")]
    public class NewChat : IReturn<NewChatResponse>
    {
        public int ParticipantID { get; set; }
    }

    [Route("/chat/newGroup")]
    public class NewGroup : IReturn<NewChatResponse>
    {
        public byte[] GroupAvatar { get; set; }
        public string GroupTitle { get; set; }
        public int[] Participants { get; set; }
    }

    [Route("/chat/deleteConversation")]
    public class DeleteConversation : IReturn<DeleteConversationResponse>
    {
        public int ConversationID { get; set; }
    }

    [Route("/chat/getConversations")]
    public class GetConversations : IReturn<GetConversationResponse> { }

    [Route("/chat/getMessages")]
    public class GetMessages : IReturn<GetMessagesResponse>
    {
        public int ConversationID { get; set; }
        public int? Limit { get; set; } = null;
        public DateTime? Before { get; set; } = null;
        public DateTime? After { get; set; } = null;
    }

    [Route("/chat/deleteMessage")]
    public class DeleteMessage : IReturn<DeleteMessageResponse>
    {
        public int MessageID { get; set; }
        public string DeleteType { get; set; }
    }

    [Route("/chat/changeNickname")]
    public class ChangeNickname : IReturn<ChangeNicknameResponse>
    {
        public int ConversationID { get; set; }
        public string Nickname { get; set; }
    }

    [Route("/chat/sendMessage")]
    public class SendMessage : IReturn<SendMessageResponse>
    {
        public int ConversationID { get; set; }
        public string Content { get; set; }
        public string MessageType { get; set; }
        public InAttachment[] Attachments { get; set; }
    }

    [Route("/chat/findChat")]
    public class FindChat : IReturn<FindChatResponse>
    {
        public int UserID { get; set; }
    }

    [Route("/chat/findConversation")]
    public class FindConversation : IReturn<FindConversationResponse>
    {
        public int ConversationID { get; set; }
    }

    public class FindConversationResponse
    {
        public ConversationResponse Conversation { get; set; }
    }

    public class FindChatResponse
    {
        public ConversationResponse Conversation { get; set; }
    }

    public class SendMessageResponse
    {
        public MessageResponse Message { get; set; }
    }

    public class ChangeNicknameResponse { }

    public class DeleteMessageResponse { }

    public class GetMessagesResponse
    {
        public MessageResponse[] Messages { get; set; }
    }

    public class GetConversationResponse
    {
        public ConversationResponse[] Conversations { get; set; }
    }

    public class DeleteConversationResponse { }

    public class NewChatResponse
    {
        public ConversationResponse Conversation { get; set; }
        //public int ConversationID { get; set; }
        //public string ConversationName { get; set; }
        //public string ConversationTitle { get; set; }
        //public int CreatorID { get; set; }
        //public string ConversationType { get; set; }
        //public string GroupType { get; set; }
        //public DateTime CreateAt { get; set; }
        //public ParticipantResponse[] Participants { get; set; }
    }

    public class ParticipantResponse
    {
        public int ParticipantID { get; set; }
        public int ConversationID { get; set; }
        public int UserID { get; set; }
        public string NickName { get; set; }
        public string ConversationRole { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ConversationResponse
    {
        public int ConversationID { get; set; }
        public string ConversationName { get; set; }
        public string ConversationTitle { get; set; }
        public int CreatorID { get; set; }
        public string ConversationType { get; set; }
        public string GroupType { get; set; }
        public DateTime CreatedAt { get; set; }

        public ParticipantResponse[] Participants { get; set; }
        public string LatestMessage { get; set; }
        public DateTime LatestMessageTime { get; set; }
    }

    public class MessageResponse
    {
        public int MessageID { get; set; }
        public UserResponse Sender { get; set; }
        public string Content { get; set; }
        public string MessageType { get; set; }
        public DateTime CreatedAt { get; set; }
        public AttachmentResponse[] Attachments { get; set; }
    }

    public class UserResponse
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
    }

    public class AttachmentResponse
    {
        public int AttachmentID { get; set; }
        public string AttachmentType { get; set; }
        public string ThumnailURL { get; set; }
        public string FileURL { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class InAttachment
    {
        public string AttachmentType { get; set; }
        public string FileName { get; set; }
        public byte[] FileData { get; set; }
    }
}

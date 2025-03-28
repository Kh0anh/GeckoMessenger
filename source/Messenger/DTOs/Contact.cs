﻿using ServiceStack;
using System;

namespace Messenger.DTOs
{

    [Route("/contact/getContacts")]
<<<<<<< HEAD
    public class GetContacts : IReturn<GetContactResponse> { }
=======
    public class GetContacts : IReturn<GetContactsResponse> { }
>>>>>>> 4ffd7326e3587c336809acb8219698eda1a94f89

    [Route("/contact/addContact")]
    public class AddContact : IReturn<AddContactResponse>
    {
        public int UserID { get; set; }
    }

    [Route("/contact/deleteContact")]
    public class DeleteContact : IReturn<DeleteContactResponse>
    {
        public int UserID { get; set; }
    }

    [Route("/contact/blockContact")]
    public class BlockContact : IReturn<BlockContactResponse>
    {
        public int UserID { get; set; }
    }

    [Route("/contact/unblockContact")]
    public class UnblockContact : IReturn<UnblockContactResponse>
    {
        public int UserID { get; set; }
    }

    [Route("/contact/getSuggestContact")]
    public class GetSuggestContact : IReturn<GetSuggestContactResponse> { }

    public class GetSuggestContactResponse
    {
        public ContactSugggestResponse[] Contacts { get; set; }
    }

    public class UnblockContactResponse { }

    public class BlockContactResponse { }

    public class AddContactResponse { }

    public class DeleteContactResponse { }

    public class GetContactsResponse
    {
        public ContactResponse[] Contacts { get; set; }
    }

    public class ContactResponse
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
        public DateTime AddedAt { get; set; }
    }

    public class ContactSugggestResponse
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
    }
}

using APIServer.DTOs;
using APIServer.Models;
using ServiceStack;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace APIServer.Services
{
    public class ContactService : Service
    {
        public IDbConnectionFactory DB { get; set; }

        public object Get(DTOs.GetContacts request)
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
                var contacts = db.Select<Contacts>(c => c.ContactID == userID && c.BlockAt == null);


                List<ContactResponse> contactResponses = new List<ContactResponse>();
                foreach (var contact in contacts)
                {
                    var userInfo = db.Single<Users>(u => u.UserID == contact.UserID);

                    contactResponses.Add(new ContactResponse
                    {
                        UserID = userInfo.UserID,
                        Username = userInfo.Username,
                        FirstName = userInfo.FirstName,
                        LastName = userInfo.LastName,
                        Avatar = userInfo.Avatar,
                        AddedAt = contact.AddedAt,
                        BlockAt = contact.BlockAt,
                    });
                }

                return new HttpResult(new GetContactsResponse { Contacts = contactResponses.ToArray() }, HttpStatusCode.OK);
            }
        }

        public object Get(DTOs.GetBlockContacts request)
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
                var contacts = db.Select<Contacts>(c => c.ContactID == userID && c.BlockAt != null);


                List<ContactResponse> contactResponses = new List<ContactResponse>();
                foreach (var contact in contacts)
                {
                    var userInfo = db.Single<Users>(u => u.UserID == contact.UserID);

                    contactResponses.Add(new ContactResponse
                    {
                        UserID = userInfo.UserID,
                        Username = userInfo.Username,
                        FirstName = userInfo.FirstName,
                        LastName = userInfo.LastName,
                        Avatar = userInfo.Avatar,
                        AddedAt = contact.AddedAt,
                        BlockAt = contact.BlockAt,
                    });
                }

                return new HttpResult(new GetBlockContactsResponse { Contacts = contactResponses.ToArray() }, HttpStatusCode.OK);
            }
        }

        public object Post(DTOs.AddContact request)
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
                var contact = db.Single<Contacts>(c => c.ContactID == userID && c.UserID == request.UserID);
                if (contact != null)
                {
                    return new HttpResult(new AddContactResponse { }, HttpStatusCode.OK);
                }

                var newContact = new Contacts
                {
                    ContactID = userID,
                    UserID = request.UserID,
                };

                db.Insert(newContact);
                return new HttpResult(new AddContactResponse { }, HttpStatusCode.OK);
            }
        }

        public object Post(DTOs.DeleteContact request)
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
                var contact = db.Single<Contacts>(c => c.ContactID == userID && c.UserID == request.UserID);

                if (contact == null)
                {
                    return new HttpResult(new { Error = "ContactNotFound", Message = "Contact does not exist." })
                    {
                        StatusCode = HttpStatusCode.OK
                    };
                }

                db.Delete(contact);

                return new HttpResult(new DeleteContact { }, HttpStatusCode.OK);
            }
        }

        public object Post(DTOs.BlockContact request)
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
                var contact = db.Single<Contacts>(c => c.ContactID == userID && c.UserID == request.UserID);

                if (contact == null)
                {
                    contact = new Contacts
                    {
                        ContactID = userID,
                        UserID = request.UserID,
                    };

                    db.Insert(contact);

                    contact = db.Single<Contacts>(c => c.ContactID == userID && c.UserID == request.UserID);
                }

                contact.BlockAt = DateTime.Now;

                db.Update(contact);
                return new HttpResult(new BlockContact { }, HttpStatusCode.OK);
            }
        }

        public object Post(DTOs.UnblockContact request)
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
                var contact = db.Single<Contacts>(c => c.ContactID == userID && c.UserID == request.UserID);

                if (contact == null)
                {
                    return new HttpResult(new { Error = "ContactNotFound", Message = "Contact does not exist." })
                    {
                        StatusCode = HttpStatusCode.OK
                    };
                }

                contact.BlockAt = null;

                db.Update(contact);
                return new HttpResult(new UnblockContact { }, HttpStatusCode.OK);
            }
        }

        public object Get(DTOs.GetSuggestContact request)
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
                var sql = @"
    SELECT TOP 10 * 
    FROM Users 
    WHERE UserID NOT IN (SELECT UserID FROM Contacts WHERE ContactID = @contactID) 
    ORDER BY NEWID()";

                var users = db.SqlList<Users>(sql, new { contactID = userID });

                List<ContactSugggestResponse> contactSuggestRespones = new List<ContactSugggestResponse>();
                foreach (var user in users)
                {
                    contactSuggestRespones.Add(new ContactSugggestResponse
                    {
                        UserID = user.UserID,
                        Username = user.Username,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Avatar = user.Avatar,
                    });
                }

                return new HttpResult(new GetSuggestContactResponse { Contacts = contactSuggestRespones.ToArray() }, HttpStatusCode.OK);
            }
        }
    }
}

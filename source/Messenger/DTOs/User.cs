using ServiceStack;
using System;
using System.Collections.Generic;

namespace Messenger.DTOs
{
    // Kiểm tra thông tin tồn tại
    [Route("/user/existedInfo")]
    public class ExistedInfo : IReturn<ExistedInfoResponse>
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class ExistedInfoResponse
    {
        public string Error { get; set; }
        public string Message { get; set; }
        public bool IsExisted { get; set; }
    }

    // Lấy thông tin user
    [Route("/user/getInfo")]
    public class GetInfo : IReturn<GetInfoResponse>
    {
        public int UserID { get; set; }  // ID của user cần lấy thông tin
    }

    public class GetInfoResponse
    {
        public string Error { get; set; }
        public string Message { get; set; }
        public UserInfo Data { get; set; }
    }

    //Thông tin người dùng được lấy ra
    public class UserInfo
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime Birthday { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Bio { get; set; }
        public string Avatar { get; set; }
        public DateTime LastLogin { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // Lấy trạng thái hoạt động
    [Route("/user/activeStatus")]
    public class ActiveStatus : IReturn<ActiveStatusResponse>
    {
        public int UserID { get; set; }  // Thêm UserID vào DTO
    }

    public class ActiveStatusResponse
    {
        public string Error { get; set; }
        public string Message { get; set; }
        public bool IsActive { get; set; }  // Trạng thái hoạt động
    }

    // Cập nhật thông tin
    [Route("/user/updateInfo")]
    public class UpdateInfo : IReturn<UpdateInfoResponse>
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime Birthday { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Bio { get; set; }
    }

    public class UpdateInfoResponse
    {
        public string Error { get; set; }
        public string Message { get; set; }
    }

    // Cập nhật avatar
    [Route("/user/updateAvatar")]
    public class UpdateAvatar : IReturn<UpdateAvatarResponse>
    {
        public string Avatar { get; set; }
    }

    public class UpdateAvatarResponse
    {
        public string Error { get; set; }
        public string Message { get; set; }
        public string AvatarUrl { get; set; }
    }

    // Tìm kiếm user
    [Route("/user/searchUser")]
    public class SearchUser : IReturn<SearchUserResponse>
    {
        public string Query { get; set; } //Chẳng hạn như nhập Query = "Khanh" thì sẽ tìm kiếm tất cả user có tên là Khanh
        public int MaxResult { get; set; } //Số lượng user tối đa trả về
    }

    public class SearchUserResponse
    {
        public string Error { get; set; }
        public string Message { get; set; }
        public List<UserInfo> Users { get; set; }
    }

    //Cập nhật quyền riêng tư
    [Route("/user/updatePrivacy")]
    public class UpdatePrivacy : IReturn<UpdatePrivacyResponse>
    {
        public string ActiveStatus { get; set; }
    }

    public class UpdatePrivacyResponse
    {
        public string Error { get; set; }
        public string Message { get; set; }
    }
    [Route("/user/getPrivacy")]
    public class GetPrivacy : IReturn<GetPrivacyResponse>
    {
        public int UserID { get; set; }
    }
    public class GetPrivacyResponse
    {
        public string Error { get; set; }
        public string Message { get; set; }
        public PrivacyInfo Data { get; set; }
    }

    public class PrivacyInfo
    {
        public string ActiveStatus { get; set; }
        public string BioPrivacy { get; set; }
        public string PhoneNumberPrivacy { get; set; }
    }
}



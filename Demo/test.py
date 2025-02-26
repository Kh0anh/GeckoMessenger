import requests

BASE_URL = "http://localhost:9696/api"

# Đăng ký tài khoản
def register(username, password):
    url = f"{BASE_URL}/users/register"
    data = {"username": username, "password": password}
    response = requests.post(url, data=data)
    return response.json()

# Đăng nhập lấy token JWT
def login(username, password):
    url = f"{BASE_URL}/users/login"
    data = {"username": username, "password": password}
    response = requests.post(url, data=data)
    return response.json().get("token", None)

# Gửi tin nhắn
def send_message(token, receiver_id, content, message_type="text"):
    url = f"{BASE_URL}/messages/send"
    headers = {"Authorization": f"Bearer {token}", "Content-Type": "application/json"}
    data = {"receiverID": receiver_id, "content": content, "message_Type": message_type}
    response = requests.post(url, json=data, headers=headers)
    return response.json()

# Lấy danh sách tin nhắn
def get_messages(token):
    url = f"{BASE_URL}/messages/get"
    headers = {"Authorization": f"Bearer {token}"}
    response = requests.get(url, headers=headers)
    return response.json()

# Chạy thử các API
if __name__ == "__main__":
    username = "khanh"
    password = "123"

    # 1. Đăng ký
    #print("Đăng ký:", register(username, password))

    # 2. Đăng nhập để lấy token
    token = login(username, password)
    if token:
        print("Token:", token)

        # 3. Gửi tin nhắn
        print("Gửi tin nhắn:", send_message(token, receiver_id=2, content="Hello từ Python!"))

        # 4. Lấy danh sách tin nhắn
        print("Danh sách tin nhắn:", get_messages(token))
    else:
        print("Đăng nhập thất bại!")

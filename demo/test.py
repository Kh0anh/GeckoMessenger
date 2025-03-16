import requests

BASE_URL = "http://localhost:8080"

# Đăng ký tài khoản
def register(username, password):
    url = f"{BASE_URL}/auth/register"
    data = {"Username": username, "Password": password}
    response = requests.post(url, json=data)
    
    if response.status_code == 201:
        print(response.text)
        print("✅ Register successful")
    else:
        print("❌ Register failed:", response.status_code, response.text)

# Đăng nhập và lấy token
def login(username, password):
    url = f"{BASE_URL}/auth/login"
    data = {"Username": username, "Password": password}
    response = requests.post(url, json=data)
    
    if response.status_code == 200:
        print(response.json())
        login_response = response.json()
        token = login_response.get("Token")  # Lấy token từ phản hồi
        print("✅ Login successful. Token:", token)
        return token
    else:
        print("❌ Login failed:", response.status_code, response.text)
        return None

# Gửi request đến message/test với token
def get_message(token):
    url = f"{BASE_URL}/messages/test"
    headers = {
        "Authorization": f"Bearer {token}"  # Đưa token vào header
    }
    response = requests.get(url, headers=headers)

    print("📩 Status Code:", response.status_code)
    print("📩 Response:", response.text)

# Thực thi chương trình
if __name__ == "__main__":
    USERNAME = "admsaiwn"
    PASSWORD = "dsadw"

    register(USERNAME, PASSWORD)  # Đăng ký tài khoản
    token = login(USERNAME, PASSWORD)  # Đăng nhập để lấy token

    if token:  # Nếu có token hợp lệ thì gửi request đến /message/test
        get_message(token)
    else:
        print("❌ No valid token, cannot request /message/test")

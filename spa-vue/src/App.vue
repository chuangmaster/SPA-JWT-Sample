<template>
  <div>
    <button @click="login" v-if="!isLogin">Login with Azure AD</button>
    <button @click="showToken" v-if="isLogin">Show Token</button>
    <button @click="logout" v-if="isLogin">Logout</button>
  </div>
</template>

<script>
import { PublicClientApplication, InteractionRequiredAuthError } from "@azure/msal-browser";

// Azure AD 和 MSAL 配置
const msalConfig = {
  auth: {
    clientId: "fa65525a-fd35-496b-abef-a1bb7e8e8edf",
    authority: "https://login.microsoftonline.com/5571c7d4-286b-47f6-9dd5-0aa688773c8e",
    redirectUri: "http://localhost:8080",
  },
};

const loginRequest = {
  scopes: ["User.Read"],
};

// 創建 MSAL 實例
const msalInstance = new PublicClientApplication(msalConfig);

export default {
  data() {
    return {
      isLogin: false,
      accessToken: "", // 儲存獲取的 access token
    };
  },
  async mounted() {
    // 初始化並等待 MSAL 實例完成初始化
    await msalInstance.initialize();
    console.log("Initializing handleRedirectPromise...");

    // 處理重定向並獲取 token
    this.handleRedirect();
  },
  methods: {
    // 登入方法
    login() {
      msalInstance.loginRedirect(loginRequest).catch((error) => {
        console.error("Login error:", error);
      });
    },

    // 處理重定向並取得 token
    async handleRedirect() {
      try {
        const response = await msalInstance.handleRedirectPromise();
        if (response) {
          console.log("Login successful:", response);
          this.accessToken = response.accessToken; // 獲取 access token
          this.isLogin = true;
          console.log("Access Token:", this.accessToken);
          alert(`Access Token: ${this.accessToken}`);
        } else {
          console.log("No response received from redirect.");
        }
      } catch (error) {
        console.error("Error handling redirect:", error);
      }
    },

    // 獲取 token 方法，用於其他 API 請求
    async acquireToken() {
      try {
        const account = msalInstance.getActiveAccount();
        if (!account) throw new Error("No active account!");

        const response = await msalInstance.acquireTokenSilent({
          ...loginRequest,
          account,
        });
        this.accessToken = response.accessToken;
        console.log("Acquired Token:", this.accessToken);
      } catch (error) {
        if (error instanceof InteractionRequiredAuthError) {
          msalInstance.acquireTokenRedirect(loginRequest);
        } else {
          console.error("Token acquisition error:", error);
        }
      }
    },

    // 顯示 token
    showToken() {
      alert(this.accessToken ? `Access Token: ${this.accessToken}` : "No token found.");
    },

    // 登出並清除狀態
    logout() {
      msalInstance.logoutRedirect();
      this.isLogin = false;
      this.accessToken = "";
    },
  },
};
</script>

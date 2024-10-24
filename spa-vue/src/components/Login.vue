<template>
    <div class="login" v-if="!isLogin">
        <h2>Login</h2>
        <button type="button" v-on:click="azLogin">Login With AZ</button>
        <button type="button" v-on:click="azLogout">Logout AZ</button>

    </div>
    <button type="button" class="logout" v-if="isLogin" @click="logout">Log out</button>
    
    <WeatherForecast v-if="isLogin"></WeatherForecast>
</template>

<script>
import axios from 'axios';
import WeatherForecast from './WeatherForecast.vue';
import { PublicClientApplication } from "@azure/msal-browser"; // if using CDN, 'Msal' will be available in global scope



export default {
    components: {
        WeatherForecast
    },
    data() {
        return {
            username: 'admin',
            password: 'password',
            platform: 'A',
            isLogin: false,
            msalInstance: null,
            accountId: ''
        };
    },
    async created() {
        this.isLogin = localStorage.getItem('isLogin') === 'true'; // Check login status from localStorage

        const config = {
            auth: {
                clientId: "22d34a7e-90d9-40fe-be89-efc1a9b13cfb",
                redirectUri: "http://localhost:8080/welcome", //defaults to application start page
                postLogoutRedirectUri: "/",
            },
        };
        this.msalInstance = new PublicClientApplication(config);
        await this.msalInstance.initialize();
    },
    methods: {
        azLogin() {
            this.msalInstance.handleRedirectPromise().then(this.handleResponse);

        },
        handleResponse(response) {
            if (response !== null) {
                this.accountId = response.account.homeAccountId;
                // Display signed-in user content, call API, etc.
            } else {
                // In case multiple accounts exist, you can select
                const currentAccounts = this.msalInstance.getAllAccounts();

                if (currentAccounts.length === 0) {
                    alert("No accounts detected. Enter your credentials.");
                    // no accounts signed-in, attempt to sign a user in
                    this.msalInstance.loginRedirect(loginRequest);
                } else if (currentAccounts.length > 1) {
                    alert("Multiple accounts detected. Please select your account.");
                    // Add choose account code here
                } else if (currentAccounts.length === 1) {
                    alert("One account detected. Continuing with the account: " + currentAccounts[0].username);
                    this.accountId = currentAccounts[0].homeAccountId;
                }
            }
        },
        azLogout() {


            // you can select which account application should sign out
            const logoutRequest = {
                account: this.msalInstance.getAccountByHomeId(this.accountId),
            };

            this.msalInstance.logoutRedirect(logoutRequest);
        }
    }
};
</script>

<style scoped>
.login {
    max-width: 300px;
    margin: 0 auto;
    padding: 20px;
    border: 1px solid #ccc;
    border-radius: 4px;
}

.login h2 {
    text-align: center;
}

.login form {
    display: flex;
    flex-direction: column;
}

.login form div {
    margin-bottom: 10px;
}

.login label {
    font-weight: bold;
}

.login input[type="text"],
.login input[type="password"] {
    padding: 5px;
    border: 1px solid #ccc;
    border-radius: 4px;
}

.login button {
    padding: 10px;
    background-color: #4caf50;
    color: white;
    border: none;
    border-radius: 4px;
    cursor: pointer;
}

.form-group {
    display: flex;
    justify-content: space-between;
}

button.logout {
    padding: 10px;
    background-color: #087087;
    color: white;
    border: none;
    border-radius: 4px;
    cursor: pointer;
}
</style>
<template>
    <div class="login" v-if="!isLogin">
        <h2>Login</h2>
        <div id="login-form">
            <form @submit.prevent="login">
                <div class="form-group">
                    <label for="username">Username:</label>
                    <input type="text" id="username" v-model="username" required>
                </div>
                <div class="form-group">
                    <label for="password">Password:</label>
                    <input type="password" id="password" v-model="password" required>
                </div>
                <div class="form-group">
                    <label for="platform">Platform</label>
                    <select class="form-select" name="platform" v-model="platform">
                        <option selected value="A">Platform A</option>
                        <option value="B">Platform B</option>
                    </select>
                </div>
                <button type="submit">Login</button>


            </form>
        </div>
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
        login() {
            const formData = new FormData();
            formData.append('username', this.username);
            formData.append('password', this.password);
            formData.append('platform', this.platform);

            axios.post('https://localhost:7173/login', formData, { withCredentials: true })
                .then(response => {
                    if (response.status === 200) {
                        this.isLogin = true;
                        localStorage.setItem('isLogin', 'true'); // Save login status to localStorage

                    }
                }).catch(error => {
                    console.log(error);
                });
        },
        logout() {
            //delete localStorage item isLogin
            this.isLogin = false;
            localStorage.removeItem('isLogin');
        },
        azLogin() {
            const loginRequest = {
                scopes: ["User.ReadWrite"],
            };
            this.msalInstance.loginPopup(loginRequest)
                .then(response => {
                    console.log(response);
                    this.accountId = response.account.accountIdentifier;
                    window.location.href = '/welcome';
                })
                .catch(error => {
                    console.log(error);
                });
        },
        azLogout() {
            this.msalInstance.logout();
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
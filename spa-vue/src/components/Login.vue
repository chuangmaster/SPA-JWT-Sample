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
                <button type="submit">Login</button>
            </form>
        </div>
    </div>
    <WeatherForecast v-else></WeatherForecast>
</template>

<script>
import axios from 'axios';
import WeatherForecast from './WeatherForecast.vue';

export default {
    components: {
        WeatherForecast
    },
    data() {
        return {
            username: 'admin',
            password: 'password',
            isLogin: false
        };
    },
    created() {
        this.isLogin = localStorage.getItem('isLogin') === 'true'; // Check login status from localStorage
    },
    methods: {
        login() {
            const formData = new FormData();
            formData.append('username', this.username);
            formData.append('password', this.password);
            axios.post('https://localhost:7173/login', formData, { withCredentials: true }).then(response => {
                if (response.status === 200) {
                    this.isLogin = true;
                    localStorage.setItem('isLogin', 'true'); // Save login status to localStorage

                }
            }).catch(error => {
                console.log(error);
            });
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
</style>
 import { createRouter, createWebHistory } from 'vue-router';
 import HelloWorld from '../components/HelloWorld.vue';
 import TheWelcome from '../components/TheWelcome.vue';
 import Login from '../components/Login.vue';

 const routes = [
     { path: '/', component: HelloWorld },
     { path: '/welcome', component: TheWelcome },
     { path: '/login', component: Login },
 ];

 const router = createRouter({
     history: createWebHistory(),
     routes,
 });

 export default router;
    
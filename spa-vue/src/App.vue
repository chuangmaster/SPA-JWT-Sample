<script setup lang="ts">
import HelloWorld from './components/HelloWorld.vue'
import Login from './components/Login.vue';

</script>

<template>
  <!--<header>
    <img alt="Vue logo" class="logo" src="./assets/logo.svg" width="125" height="125" />
    <div class="wrapper">-->
      <!--<HelloWorld msg="You did it!" />-->

    <!--</div>
  </header>-->

  <main>
      <nav>
          <RouterLink to="/">Go to Home</RouterLink> |
          <RouterLink to="/welcome">Go to Welcome</RouterLink> |
          <RouterLink to="/login">Go to Login</RouterLink>
      </nav>
      <router-view/>
  </main>

</template>

<style scoped>
header {
  line-height: 1.5;
}

.logo {
  display: block;
  margin: 0 auto 2rem;
}

@media (min-width: 1024px) {
  header {
    display: flex;
    place-items: center;
    padding-right: calc(var(--section-gap) / 2);
  }

  .logo {
    margin: 0 2rem 0 0;
  }

  header .wrapper {
    display: flex;
    place-items: flex-start;
    flex-wrap: wrap;
  }
}
</style>

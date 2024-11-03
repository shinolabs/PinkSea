<script setup lang="ts">

import BreadCrumbBar from '@/components/BreadCrumbBar.vue'
import LoginBar from '@/components/LoginBar.vue'
</script>

<template>
  <div class="container">
    <aside>
    </aside>
    <main>
      <section>
        <BreadCrumbBar />
        <slot></slot>
      </section>
    </main>
    <aside>
      <h1>PinkSea</h1>
      <div>oekaki BBS</div>
      <div>
        <div>Login to start creating!</div>
        <LoginBar />
      </div>
    </aside>
  </div>
</template>

<style scoped>
  .container {
    display: flex;
    flex-direction: row;
    justify-content: center;
  }
  .container aside {
    max-width: 20rem;
    width: 100%;
    display: block;
  }
  .container main {
    max-width: 42rem;
    width: 100%;
    display: block;
  }
  .container main section {
    min-height: 100vh;
  }

  @media (max-width: 768px) {
    .container main {
      text-align: center;
    }
    .container aside {
      display: none;
    }
  }
</style>

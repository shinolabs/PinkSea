<script setup lang="ts">
  import { ref } from 'vue'

  const handle = ref<string>("");
  const beginOAuth = () => {
    document.location = `http://localhost:5084/oauth/login?handle=${handle.value}`;
  };
</script>

<template>
  <div>
    <input type="text" placeholder="@your.cool.tld" v-model="handle">
    <button v-on:click.prevent="beginOAuth">@ Login</button>
  </div>
</template>

<style scoped>

</style>

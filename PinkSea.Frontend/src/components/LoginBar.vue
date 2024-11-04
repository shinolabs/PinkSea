<script setup lang="ts">
import { ref } from 'vue'

const handle = ref<string>('')
const beginOAuth = () => {
  document.location = `http://localhost:5084/oauth/login?handle=${handle.value}&redirectUrl=`
}
</script>

<template>
  <div>
    <input type="text" placeholder="@your.cool.tld" v-model="handle">
    <button v-on:click.prevent="beginOAuth">@ Login</button>
  </div>
</template>

<style scoped>
input, button {
  border: 1px solid #FFB6C1;
}

button {
  background-color: #FFB6C1;
  font-weight: bold;
  letter-spacing: -0.7px;
  border-style: outset;
}

button:hover {
  background-color: #FFCCD5;
}

button:active {
  background-color: #FFA8B8;
  border-style: inset;
}
</style>

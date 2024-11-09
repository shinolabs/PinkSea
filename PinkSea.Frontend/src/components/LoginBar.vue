<script setup lang="ts">
import { ref } from 'vue'
import { serviceEndpoint } from '@/api/atproto/client'

const handle = ref<string>('')
const beginOAuth = () => {
  document.location = `${serviceEndpoint}/oauth/login?handle=${handle.value}&redirectUrl=${location.origin}/callback`
}
</script>

<template>
  <div>
    <input type="text" placeholder="@your.cool.tld" v-model="handle">
    <button v-on:click.prevent="beginOAuth">@ Login</button>
  </div>
</template>

<style scoped>

</style>

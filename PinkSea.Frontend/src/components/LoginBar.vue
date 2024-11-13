<script setup lang="ts">
import { ref } from 'vue'
import { serviceEndpoint } from '@/api/atproto/client'
import i18next from 'i18next'

const handle = ref<string>('')
const beginOAuth = () => {
  document.location = `${serviceEndpoint}/oauth/login?handle=${handle.value}&redirectUrl=${location.origin}/callback`
}
</script>

<template>
  <div>
    <input type="text" :placeholder="i18next.t('menu.input_placeholder')" v-model="handle">
    <button v-on:click.prevent="beginOAuth">{{ $t("menu.atp_login") }}</button>
  </div>
</template>

<style scoped>

</style>

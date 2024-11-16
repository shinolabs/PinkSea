<script setup lang="ts">
import { ref, useTemplateRef } from 'vue'
import i18next from 'i18next'
import { xrpc } from '@/api/atproto/client'

const handle = ref<string>('')
const password = ref<string>('');

const loginButton = useTemplateRef<HTMLButtonElement>("login-button");

const beginOAuth = async () => {
  loginButton.value!.disabled = true;
  const { data } = await xrpc.call("com.shinolabs.pinksea.beginLoginFlow", {
    data: {
      handle: handle.value,
      redirectUrl: `${location.origin}/callback`,

      password: password.value.length > 0
        ? password.value
        : null
    }
  });

  if (data.redirect !== null && data.redirect !== undefined) {
    document.location = data.redirect;
  } else {
    alert(`Failed to log in: ${data.failureReason}`);
    loginButton.value!.disabled = false;
  }
}
</script>

<template>
  <div>
    <input type="text" :placeholder="i18next.t('menu.input_placeholder')" v-model="handle">
    <input type="password" :placeholder="i18next.t('menu.password')" :title="i18next.t('menu.oauth2_info')" v-model="password">
    <br />
    <button v-on:click.prevent="beginOAuth" ref="login-button">{{ $t("menu.atp_login") }}</button>
  </div>
</template>

<style scoped>

</style>

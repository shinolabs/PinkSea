<script setup lang="ts">
import { ref, useTemplateRef } from 'vue'
import i18next from 'i18next'
import { xrpc } from '@/api/atproto/client'
import Modal from './Modal.vue';

const handle = ref<string>('')
const password = ref<string>('');

const modal = useTemplateRef("modal")
const errorReason = ref<string>("")

const loginButton = useTemplateRef<HTMLButtonElement>("login-button");

const beginOAuth = async () => {
  loginButton.value!.disabled = true;
  try {
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
    }
  } catch (e) {
    modal.value?.show();
    errorReason.value = `${e}`;
    loginButton.value!.disabled = false;
  }
}
</script>

<template>
  <div>
    <input type="text" v-on:keydown.enter="beginOAuth" :placeholder="i18next.t('menu.input_placeholder')"
      v-model="handle">
    <input type="password" v-on:keydown.enter="beginOAuth" :placeholder="i18next.t('menu.password')"
      :title="i18next.t('menu.oauth2_info')" v-model="password">
    <br />
    <button v-on:click.prevent="beginOAuth" ref="login-button">{{ $t("menu.atp_login") }}</button>
  </div>

  <Modal ref="modal">
    <template #header>
      There was an error logging in!
    </template>
    <template #body>
      <p>{{ errorReason }}</p>
      <button v-on:click="modal?.dismiss()">
        Close
      </button>
    </template>
  </Modal>
</template>

<style scoped></style>

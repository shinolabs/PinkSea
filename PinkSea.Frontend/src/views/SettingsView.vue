<script setup lang="ts">

import PanelLayout from '@/layouts/PanelLayout.vue'
import { usePersistedStore } from '@/state/store'
import i18next from 'i18next'
import I18n from '@/intl/i18n'
import { watch } from 'vue'

const persistedStore = usePersistedStore();
watch(persistedStore, () => {
  i18next.changeLanguage(persistedStore.lang);
});
</script>

<template>
  <PanelLayout>
    <div class="settings-container">
      <fieldset>
        <legend>general</legend>
        <span>Language </span>
        <select v-model="persistedStore.lang" >
          <option v-for="lang in Object.keys(I18n)" v-bind:key="lang" :value="lang">{{ lang }}</option>
        </select>
      </fieldset>
      <fieldset>
        <legend>sensitive media</legend>
        <input type="checkbox" v-model="persistedStore.hideNsfw"> <span>Blur NSFW posts</span>
      </fieldset>
    </div>
  </PanelLayout>
</template>

<style scoped>
  .settings-container {
    padding: 10px;
  }

  .settings-container fieldset {
    margin-bottom: 10px;
  }
</style>

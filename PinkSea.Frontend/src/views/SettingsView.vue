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
        <legend>{{ $t("settings.category_general") }}</legend>
        <span style="margin-right: 10px;">{{ $t("settings.general_language") }}</span>
        <select v-model="persistedStore.lang" >
          <option v-for="(lang, code) in I18n" :key="code" :value="code">{{ lang.name }}</option>
        </select>
      </fieldset>
      <fieldset>
        <legend>{{ $t("settings.category_sensitive") }}</legend>
        <input type="checkbox" v-model="persistedStore.blurNsfw"> <span>{{ $t("settings.sensitive_blur_nsfw") }}</span><br />
        <input type="checkbox" v-model="persistedStore.hideNsfw"> <span>{{ $t("settings.sensitive_hide_nsfw") }}</span>

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

  legend {
    font-weight: bold;
  }

  legend:before {
    content: ">>";
    color: #FFB6C1;
    letter-spacing: -4px;
    margin-left: -6.5px; margin-right: 4px;
  }

  fieldset {
    border: 2px solid #FFB6C1;
  }
</style>

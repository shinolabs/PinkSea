<script setup lang="ts">

import { useIdentityStore, usePersistedStore } from '@/state/store'
import type { Oekaki } from '@/models/oekaki'
import { xrpc } from '@/api/atproto/client'
import { getRecordKeyFromAtUri } from '@/api/atproto/helpers'

const props = defineProps<{
  oekaki: Oekaki
}>();

const identityStore = useIdentityStore();
const persistedStore = usePersistedStore();

const deleteOekaki = async () => {
  const confirmed = confirm("Are you sure you want to delete your oekaki? This action cannot be undone.");
  if (!confirmed) {
    return;
  }

  const rkey = getRecordKeyFromAtUri(props.oekaki.at);
  await xrpc.call("com.shinolabs.pinksea.deleteOekaki", {
    data: {
      rkey: rkey!
    },
    headers: {
      "Authorization": `Bearer ${persistedStore.token}`
    }
  });

  window.location.reload();
};
</script>

<template>
<div v-if="props.oekaki.author.did == identityStore.did">
  <div class="action-button" v-on:click.prevent="deleteOekaki">[delete]</div>
</div>
</template>

<style scoped>
  .action-button {
    color: red;
    cursor: pointer;
  }

  .action-button:hover {
    text-decoration: underline dotted;
  }
</style>

<script setup lang="ts">

import BreadCrumbBar from '@/components/BreadCrumbBar.vue'
import LoginBar from '@/components/LoginBar.vue'
import { useIdentityStore, usePersistedStore } from '@/state/store'
import { useRouter } from 'vue-router'
import { onBeforeMount } from 'vue'
import { xrpc } from '@/api/atproto/client'

const identityStore = useIdentityStore()
const persistedStore = usePersistedStore()
const router = useRouter()

const startPainting = async () => {
  await router.push('/paint')
}

onBeforeMount(async () => {
  if (persistedStore.token != null &&
    identityStore.handle == null) {
    const { data } = await xrpc.get("com.shinolabs.pinksea.getIdentity", {
      params: {},
      headers: {
        "Authorization": `Bearer ${persistedStore.token}`
      }});
    identityStore.did = data.did;
    identityStore.handle = data.handle;
  }
})
</script>

<template>
  <div class="container">
    <main>
      <section>
        <BreadCrumbBar />
        <slot></slot>
      </section>
    </main>
    <aside>
      <div class="title">
        <h1>PinkSea</h1>
        <h2>oekaki BBS</h2>
      </div>
      <div class="aside-box" v-if="persistedStore.token == null">
        <div class="prompt">Login to start creating!</div>
        <LoginBar />
      </div>
      <div class="aside-box" v-else>
        <div class="prompt">Hi @{{ identityStore.handle }}!</div>
        <button v-on:click.prevent="startPainting">Create something</button>
      </div>
      <div class="aside-box bottom">
        a shinonome laboratories project
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
  display: block;
  width: 260px;
  background-size: 8px 8px;
  background-image: repeating-linear-gradient(45deg, #FFB6C1 0, #FFB6C1 0.8px, #FFFFFF 0, #FFFFFF 50%);
  opacity: 1.0;
  border: 1px solid #FFB6C1;
  border-width: 0px 1px 0px 1px;
}

.title {
  background-color: #FFFFFF;
  background-image: radial-gradient(7rem at bottom left, #FFE5EA, transparent);
  text-align: right;
  margin-bottom: 20px;
  margin-top: 35px;
}

.container main {
  background-color: #FFFFFF;
  border-left: 1px solid #FFB6C1;
  max-width: 42rem;
  width: 100%;
  display: block;
}

.container main section {
  min-height: 100vh;
}

h1 {
  font-size: 20pt;
  margin-bottom: -10px;
  padding-top: 6px;
}

.title h2 {
  font-style: italic;
  font-weight: normal;
  padding-bottom: 6px;
}

h1, .title h2 {
  padding-right: 4px;
  padding-left: 4px;
}

.title, .aside-box {
  box-shadow: 0px 1px #FFB6C1, 0px -1px #FFB6C1;
}

.aside-box {
  background: #FFFFFF;
  margin: -10px 0px 0px 0px;
  padding: 10px 0px 10px 0px;
  text-align: center;
}

.prompt:before {
  content: "★";
  color: #FFB6C1;
  margin-right: 4px;
}

.prompt {
  margin-bottom: 4px;
  font-weight: bold;
  letter-spacing: -0.5px;
}

.bottom {
  position: absolute;
  bottom: 10px;
  font-size: smaller;
  width: inherit;
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

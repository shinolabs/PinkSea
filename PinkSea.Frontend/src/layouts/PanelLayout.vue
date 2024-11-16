<script setup lang="ts">

import BreadCrumbBar from '@/components/BreadCrumbBar.vue'
import LoginBar from '@/components/LoginBar.vue'
import { useIdentityStore, useImageStore, usePersistedStore } from '@/state/store'
import { useRouter } from 'vue-router'
import { computed, onBeforeMount, useTemplateRef } from 'vue'
import { serviceEndpoint, xrpc } from '@/api/atproto/client'
import i18next from 'i18next'

const identityStore = useIdentityStore()
const persistedStore = usePersistedStore()
const imageStore = useImageStore()
const router = useRouter()
const menuRef = useTemplateRef<HTMLElement>("menu-ref");

const selfProfileUrl = computed(() => {
  return `/${identityStore.did}`;
});

onBeforeMount(async () => {
  if (persistedStore.token != null &&
    identityStore.handle == null) {
    try {
      const { data } = await xrpc.get("com.shinolabs.pinksea.getIdentity", {
        params: {},
        headers: {
          "Authorization": `Bearer ${persistedStore.token}`
        }});

      identityStore.did = data.did;
      identityStore.handle = data.handle;
    } catch {
      persistedStore.token = null;
    }
  }

  await i18next.changeLanguage(persistedStore.lang);
});

const openPainter = async () => {
  imageStore.restartPainting = true;
  await navigateTo('/paint');
};

const navigateTo = async (url: string) => {
  await router.push(url);
};

const logout = async () => {
  try {
    await xrpc.call("com.shinolabs.pinksea.invalidateSession", {
      data: {},
      headers: {
        "Authorization": `Bearer ${persistedStore.token}`
      }});
  } finally {
    persistedStore.token = null;
  }
};

const switchMenu = () => {
  menuRef.value!.classList.toggle("in-view");
};
</script>

<template>
  <div class="container">
    <main>
      <section>
        <BreadCrumbBar class="crumb" />
        <slot></slot>
        <button class="menu-button" v-on:click.prevent="switchMenu">☰</button>
      </section>
    </main>
    <aside ref="menu-ref">
      <div class="title">
        <h1>{{ $t("sidebar.title") }}</h1>
        <h2>{{ $t("sidebar.tag") }}</h2>
      </div>
      <div class="aside-box">
        <div v-if="persistedStore.token === null">
          <div class="prompt">{{ $t("menu.invitation") }}</div>
          <LoginBar />
        </div>
        <div class="prompt" v-else>{{ $t("menu.greeting", { name: identityStore.handle }) }}</div>
        <ul class="aside-menu">
          <li v-on:click.prevent="navigateTo(selfProfileUrl)" v-if="persistedStore.token !== null">{{ $t("menu.my_oekaki") }}</li>
          <li v-on:click.prevent="navigateTo('/')">{{ $t("menu.recent") }}</li>
          <li v-on:click.prevent="navigateTo('/settings')">{{ $t("menu.settings") }}</li>
          <li v-on:click.prevent="logout" v-if="persistedStore.token !== null">{{ $t("menu.logout") }}</li>
        </ul>
        <button v-on:click.prevent="openPainter" v-if="persistedStore.token !== null">{{ $t("menu.create_something") }}</button>
      </div>
      <div class="aside-box bottom">
        {{ $t("sidebar.shinolabs") }}
      </div>
    </aside>
  </div>
</template>

<style scoped>
.aside-menu {
  display: inline;
  color: black;
  list-style: none;
}

.aside-menu li {
  border: 1px solid #FFB6C1;
  padding: 4px;
  margin: 0 4% 6px 4%;
}

.aside-menu li:hover {
  border-right-width: 8px;
  box-shadow: inset -2px 0 white, inset -4px 0 #FFB6C1;
}

.container {
  display: flex;
  flex-direction: row;
  justify-content: center;
}

.crumb {
  position: sticky;
  top: 0;
}

.container aside {
  display: unset;
  width: 260px;
  height: 100vh;
  background-size: 8px 8px;
  background-image: repeating-linear-gradient(45deg, #FFB6C1 0, #FFB6C1 0.8px, #FFFFFF 0, #FFFFFF 50%);
  opacity: 1.0;
  position: sticky;
  top: 0;
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

.container main .menu-button {
  display: none;
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

.aside-box ul {
  text-align: left;
}

.aside-box ul li:hover {
  text-decoration: underline dotted;
  cursor: pointer;
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
  .container main .menu-button {
    display: block;
    position: fixed;
    bottom: 20px;
    right: 20px;

    padding: 10px;

    z-index: 999;
  }

  .container main {
    text-align: center;
  }

  .container aside {
    position: fixed;
    top: 0;
    z-index: 10;
    left: 100vw;
    transition: left 0.1s ease-out;
  }

  .in-view {
    transition: left 0.1s ease-out;
    left: calc(100vw - 260px) !important;
  }
}
</style>

<script setup lang="ts">
import { useBreadcrumbBarStore } from '@/api/breadcrumb/store'
import { resolveCrumb } from '@/api/breadcrumb/breadcrumb'
import { onBeforeMount, ref } from 'vue'
import i18next from 'i18next'

const forceRerenderKey = ref<number>(0);

const store = useBreadcrumbBarStore();

onBeforeMount(() => {
  i18next.on('languageChanged', () => {
    forceRerenderKey.value += 1
  });
})
</script>

<template>
  <div class="bar">
    <RouterLink to="/" class="bar-current link">PinkSea</RouterLink>
    <span :key="forceRerenderKey">
      <span v-for="crumb of store.crumbs" v-bind:key="crumb.path">
        <span> &gt; </span>
        <RouterLink :to="crumb.path" class="link">{{ resolveCrumb(crumb) }}</RouterLink>
      </span>
    </span>
  </div>
</template>

<style scoped>
.bar {
  margin: 0px 0px 0px 0px;
  background-color: #FFF5F6;
  border-bottom: 1px dashed #FFB6C1;
  padding: 10px 34px 8px 10px;
  z-index: 10;
}

.bar * {
  text-decoration: none;
  color: black;
}

.link:hover {
  text-decoration: underline;
  cursor: pointer;
}

.bar:before {
  content: "》》》";
  letter-spacing: -3px;
  margin-right: 8px;
  color: #FFB6C1;
  font-weight: bold;
}

.bar-current {
  font-weight: bold;
}

@media (max-width: 768px) {
  .bar {
    text-align: center;
  }
}
</style>

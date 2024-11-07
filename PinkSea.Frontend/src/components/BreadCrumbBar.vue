<script setup lang="ts">
  import { useBreadcrumbBarStore } from '@/api/breadcrumb/store'

  const store = useBreadcrumbBarStore();
</script>

<template>
  <div class="bar">
    <RouterLink to="/" class="bar-current link">PinkSea</RouterLink>
    <span>
      <span v-for="crumb of store.crumbs" v-bind:key="crumb.name">
        <span> &gt; </span>
        <RouterLink :to="crumb.path" class="link">{{ crumb.name }}</RouterLink>
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
</style>

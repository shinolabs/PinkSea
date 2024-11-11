<script setup lang="ts">
import { useRouter } from 'vue-router'

const props = defineProps<{
  tags: string[],
  disableNavigation?: boolean
}>();

const router = useRouter();

const navigateToTag = async (tag: string) => {
  if (props.disableNavigation) {
    return;
  }
  await router.push(`/tag/${tag}`);
};

</script>

<template>
  <div class="oekaki-tag-container">
    <span class="oekaki-tag" v-for="tag of props.tags" v-bind:key="tag" v-on:click.prevent="navigateToTag(tag)">#{{ tag }}</span>
  </div>
</template>

<style scoped>
.oekaki-tag-container {
  display: flex;
  justify-content: left;
  margin-top: 10px;
  overflow: clip;
}

.oekaki-tag {
  margin-right: 5px;
  padding: 5px;
  background-color: #FFB6C1;
  border-radius: 4px;
  color: #263B48;
}

.oekaki-tag:hover {
  text-decoration: underline dotted;
  cursor: pointer;
}
</style>

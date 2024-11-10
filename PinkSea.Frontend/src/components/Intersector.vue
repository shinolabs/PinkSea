<script setup lang="ts">
import { onBeforeUnmount, onMounted, useTemplateRef } from 'vue'

const emit = defineEmits(["intersected"]);
const trigger = useTemplateRef<HTMLSpanElement>("intersector-ref");

const handleIntersection = (entry: IntersectionObserverEntry) => {
  console.log("abcd");
  if (entry.isIntersecting) {
    emit('intersected');
  }
};

let observer: IntersectionObserver;
onMounted(() => {
  observer = new IntersectionObserver(entries => {
    handleIntersection(entries[0]);
  });

  observer.observe(trigger.value as Element);
});

onBeforeUnmount(() => {
  observer.disconnect();
})
</script>

<template>
  <span ref="intersector-ref"></span>
</template>

<style scoped>

</style>

<script setup lang="ts">
import { computed } from 'vue';

const props = defineProps<{
    image?: string,
    size?: number
}>()

const avatarUrl = computed(() => {
    return props.image ?? "/assets/img/blank_avatar.png"
})

const avatarSize = computed(() => {
    return props.size ?? 128
})

const avatarStyle = computed(() => {
    return {
        minWidth: avatarSize.value + 'px',
        minHeight: avatarSize.value + 'px',
        maxWidth: avatarSize.value + 'px',
        maxHeight: avatarSize.value + 'px',
        background: `url(${avatarUrl.value})`,
        backgroundSize: "cover",
        backgroundPosition: "center",
        backgroundRepeat: "no-repeat",
        display: 'inline-block'
    }
})

</script>

<template>
    <div :style="avatarStyle"></div>
</template>

<style scoped></style>
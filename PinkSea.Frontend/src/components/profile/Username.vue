<script lang="ts" setup>
import type { Author } from '@/models/author';
import { UsernameDisplayType } from '@/models/username-display-type';
import { usePersistedStore } from '@/state/store';
import { computed } from 'vue';

const store = usePersistedStore()

const props = defineProps<{
    author: Author
}>()

const username = computed(() => {
    if (props.author.nickname === undefined) {
        return `@${props.author.handle}`
    }

    if (store.usernameDisplayType == UsernameDisplayType.NicknameWithHandle) {
        return `${props.author.nickname}`
    } else if (store.usernameDisplayType == UsernameDisplayType.OnlyNickname) {
        return props.author.nickname
    } else {
        return `@${props.author.handle}`
    }
})

const displayHandle = computed(() => {
    return store.usernameDisplayType == UsernameDisplayType.NicknameWithHandle &&
        props.author.nickname !== undefined
})

</script>

<template>
    <span>
        {{ username }}<span v-if="displayHandle" class="handle"> (@{{ props.author.handle }})</span>
    </span>
</template>

<style scoped>
.handle {
    font-size: smaller;
}
</style>
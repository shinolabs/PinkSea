<script setup lang="ts">
import { formatDate } from '@/api/atproto/helpers';
import type { Oekaki } from '@/models/oekaki';
import { computed } from 'vue';
import TagContainer from '../TagContainer.vue';
import PostActions from '../PostActions.vue';
import Avatar from '../profile/Avatar.vue';
import Username from '../profile/Username.vue';

const props = defineProps<{
    oekaki: Oekaki
    showPostActions?: boolean
    showSubstituteOnNoTags?: boolean
}>();

const authorProfileLink = computed(() => `/${props.oekaki.author.did}`);
const creationTime = computed(() => {
    return formatDate(props.oekaki.creationTime)
})

</script>

<template>
    <div class="oekaki-meta">
        <div class="oekaki-user-info-container">
            <div>
                <Avatar :image="props.oekaki.author.avatar" :size="42" />
            </div>
            <div class="oekaki-handle-container">
                <span>{{ $t("timeline.by_before_handle") }}<b class="oekaki-author">
                        <RouterLink :to="authorProfileLink">
                            <Username :author='props.oekaki.author' />
                        </RouterLink>
                    </b>{{ $t("timeline.by_after_handle") }}</span><br>
                <span>{{ creationTime }}</span><br>
            </div>
        </div>
        <PostActions :oekaki="props.oekaki" v-if="props.showPostActions" />
        <TagContainer v-if="props.oekaki.tags !== undefined && props.oekaki.tags.length > 0"
            :tags="props.oekaki.tags" />
        <div class="oekaki-tag-container-substitute" v-else-if="props.showSubstituteOnNoTags">.</div>
    </div>
</template>

<style scoped>
.oekaki-tag-container-substitute {
    margin-top: 10px;
    padding: 5px;
    visibility: hidden;
}

.oekaki-author {
    text-decoration: underline dotted;
}

.oekaki-author:hover {
    text-decoration: underline;
    cursor: pointer;
}

.oekaki-user-info-container {
    display: flex;
    align-items: center;
    overflow: hidden;
    word-wrap: break-word;
}

.oekaki-handle-container {
    margin-left: 10px;
}

.oekaki-meta {
    font-size: small;
    padding: 10px;
    color: #2f4858;
    border-top: 2px dashed #FFB6C1;
    border-left: 0.525em solid #FFB6C1;
}
</style>
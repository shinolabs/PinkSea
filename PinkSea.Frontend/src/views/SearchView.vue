<script setup lang="ts">
import { SearchType } from '@/models/search-type'
import { ref } from 'vue'
import PanelLayout from '@/layouts/PanelLayout.vue'
import TimeLine from '@/components/TimeLine.vue'
import type { TagSearchResult } from '@/models/tag-search-result'
import type { Author } from '@/models/author'
import { xrpc } from '@/api/atproto/client'
import { useRoute, useRouter } from 'vue-router'
import Intersector from '@/components/Intersector.vue'
import SearchTag from '@/components/search/SearchTag.vue'
import SearchProfileCard from '@/components/search/SearchProfileCard.vue'

const tabs = [
  {
    i18n: "search.posts_tab",
    id: SearchType.Posts
  },
  {
    i18n: "search.tags_tab",
    id: SearchType.Tags
  },
  {
    i18n: "search.profiles_tab",
    id: SearchType.Profiles
  },
];

const currentTab = ref<SearchType>(SearchType.Posts);

const tags = ref<TagSearchResult[]>([]);
const profiles = ref<Author[]>([]);

const route = useRoute();
const router = useRouter();

const applyNew = (resp: {
  tags?: TagSearchResult[] | null
  profiles?: Author[] | null
}) => {
  if (currentTab.value == SearchType.Tags) {
    tags.value = tags.value.concat(resp.tags!);
  } else if (currentTab.value == SearchType.Profiles) {
    profiles.value = profiles.value.concat(resp.profiles!);
  }
}

const open = async (id: string) => {
  let url = ""
  if (currentTab.value == SearchType.Tags) {
    url = `/tag/${id}`;
  } else if (currentTab.value == SearchType.Profiles) {
    url = `/${id}`;
  }

  await router.push(url);
};

const loadMore = async () => {
  const opts = { params: { query: route.params.value, type: currentTab.value } };

  // @ts-expect-error It's good.
  const { data } = await xrpc.get("com.shinolabs.pinksea.getSearchResults", opts);

  applyNew(data);
};

const setTab = (tab: SearchType) => {
  currentTab.value = tab;

  if (currentTab.value == SearchType.Tags) {
    tags.value = [];
  } else if (currentTab.value == SearchType.Profiles) {
    profiles.value = [];
  }
};
</script>

<template>
  <PanelLayout>
    <div id="tabs">
      <a v-for="tab in tabs" :class="tab.id == currentTab ? 'selected' : ''" v-on:click.prevent="setTab(tab.id)"
        v-bind:key="tab.id">{{ $t(tab.i18n) }}</a>
    </div>
    <div v-if="currentTab == SearchType.Posts">
      <TimeLine endpoint="com.shinolabs.pinksea.getSearchResults"
        :xrpc-params="{ query: $route.params.value, type: SearchType.Posts }" />
    </div>
    <div v-else-if="currentTab == SearchType.Tags" class="search-result-list">
      <div v-if="tags.length > 0" v-for="tag of tags" :key="tag.tag">
        <SearchTag :tag="tag" v-on:click.prevent="open(tag.tag)" />
      </div>
      <div v-else class="search-centered">
        {{ $t("timeline.nothing_here") }}
      </div>
      <Intersector @intersected="loadMore" />
    </div>
    <div v-else-if="currentTab == SearchType.Profiles" class="search-result-list">
      <div v-if="profiles.length > 0" v-for="profile of profiles" :key="profile.did">
        <SearchProfileCard :profile="profile" v-on:click.prevent="open(profile.did)" />
      </div>
      <div v-else class="search-centered">
        {{ $t("timeline.nothing_here") }}
      </div>
      <Intersector @intersected="loadMore" />
    </div>
  </PanelLayout>
</template>

<style scoped>
#tabs {
  position: relative;
  display: flex;
  margin: 20px 0px 0px;
  height: 23px;
  box-shadow: 0px 3px #ffffff, 0px 4px #ffb6c1;
}

#tabs a {
  flex: 1;
  color: #0085ff;
  vertical-align: middle;
  display: inline-block;
  text-decoration: none;
  text-align: center;
  height: 20px;
  padding-top: 3px;
  cursor: pointer;
  border-bottom: 1px solid #ffb6c1;
}

#tabs a.selected {
  background: #ffb6c1;
  color: #263b48;
  font-weight: bold;
}

#tabs a:hover:not(.selected) {
  background: #ffb6c140;
}

.search-result-list {
  padding: 10px;
}

.search-result-list>div {
  cursor: pointer;
}

.search-centered {
  text-align: center;
}
</style>

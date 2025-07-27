<script setup lang="ts">
import { onMounted, ref, useTemplateRef, watch } from 'vue'
import { Tegaki } from '@/api/tegaki/tegaki';
import { useRouter } from 'vue-router'
import PanelLayout from '@/layouts/PanelLayout.vue'
import { useIdentityStore, useImageStore, usePersistedStore } from '@/state/store'
import { xrpc } from '@/api/atproto/client'
import TagContainer from '@/components/TagContainer.vue'
import i18next from 'i18next'

const persistedStore = usePersistedStore();
const identityStore = useIdentityStore();
const imageStore = useImageStore();
const image = ref<string>(imageStore.lastDoneImage ?? "");
const router = useRouter();

const bsky = ref<boolean>(false);
const nsfw = ref<boolean>(false);
const currentTag = ref<string>("");
const alt = ref<string>("");
const tags = ref<string[]>([]);

const button = useTemplateRef<HTMLButtonElement>("upload-button");

onMounted(() => {
  if (image.value !== "" && !imageStore.restartPainting)
    return;

  if (imageStore.lastUploadErrored) {
    if (confirm(i18next.t("painter.do_you_want_to_restore")))
      return;
  }

  openTegaki();
});

watch(imageStore, () => {
  if (imageStore.restartPainting) {
    if (imageStore.lastUploadErrored) {
      if (confirm(i18next.t("painter.do_you_want_to_restore")))
        return;
    }

    openTegaki();
  }
});

const openTegaki = () => {
  imageStore.restartPainting = false;
  imageStore.lastUploadErrored = false;

  try {
    Tegaki.destroy();
  } catch {

  } finally {
    Tegaki.open({
      onDone: () => {
        image.value = Tegaki.flatten().toDataURL("image/png");
        imageStore.lastDoneImage = image.value;
      },

      onCancel: () => {
        router.push('/');
      },

      width: 400,
      height: 400
    });
  }
};

const edit = () => {
  Tegaki.open({
    onDone: () => {
      image.value = Tegaki.flatten().toDataURL("image/png");
      imageStore.lastDoneImage = image.value;
    }
  });
};

const uploadImage = async () => {
  button.value!.disabled = true;

  try {
    // Force refresh the session, just to be sure.
    await xrpc.call("com.shinolabs.pinksea.refreshSession", {
      data: {},
      headers: {
        "Authorization": `Bearer ${persistedStore.token}`
      }
    });
  } catch {
    button.value!.disabled = false;
    imageStore.lastUploadErrored = true;
    persistedStore.token = null;

    alert(i18next.t("painter.your_session_has_expired"));

    await router.push('/');

    return;
  }

  try {
    const { data } = await xrpc.call("com.shinolabs.pinksea.putOekaki", {
      data: {
        data: image.value,
        tags: tags.value,
        nsfw: nsfw.value,
        alt: alt.value,
        parent: undefined,
        bskyCrosspost: bsky.value
      },
      headers: {
        "Authorization": `Bearer ${persistedStore.token}`
      }
    });

    imageStore.lastDoneImage = null;
    imageStore.lastUploadErrored = false;

    await router.push(`/${identityStore.did}/oekaki/${data.rkey}`);
  } catch {
    button.value!.disabled = false;
    imageStore.lastUploadErrored = true;

    alert(i18next.t("painter.could_not_send_post"));
  }
};

const addTag = () => {
  if (currentTag.value.trim().length < 1)
    return;

  if (tags.value.length > 5)
    return;

  tags.value.push(currentTag.value.toLowerCase());
  currentTag.value = "";
};

const removeTag = () => {
  if (currentTag.value.length > 0)
    return;

  if (tags.value.length < 1)
    return;

  tags.value.pop();
};
</script>

<template>
  <PanelLayout>
    <div class="image-container">
      <img v-bind:src="image" />
    </div>
    <br />
    <table class="response-extra">
      <tbody>
        <tr>
          <td><b>{{ $t("painter.upload_description") }}</b></td>
          <td>
            <div class="response-items">
              <input type="text" v-model="alt" :placeholder="i18next.t('painter.add_a_description')" />
              <p class="response-hint">{{ $t("painter.hint_description") }}</p>
            </div>
          </td>
        </tr>
        <tr>
          <td><b>{{ $t("painter.upload_tags") }}</b></td>
          <td>
            <div class="response-items">
              <div class="tag-input">
                <TagContainer :tags="tags" :disableNavigation="true" />
                <input type="text" :placeholder="i18next.t('painter.tag')" v-model="currentTag"
                  v-on:keyup.delete="removeTag" v-on:keyup.space="addTag" v-on:keyup.enter="addTag" />
              </div>
              <p class="response-hint">{{ $t("painter.hint_tags") }}</p>
            </div>
          </td>
        </tr>
        <tr>
          <td><b>{{ $t("painter.upload_social") }}</b></td>
          <td>
            <div class="response-items">
              <input type="checkbox" value="nsfw" v-model="nsfw"><span>NSFW</span><br />
              <p class="response-hint">{{ $t("painter.hint_nsfw") }}</p>
              <input type="checkbox" value="bsky" v-model="bsky"><span>{{ $t("painter.crosspost_to_bluesky") }}</span>
              <p class="response-hint">{{ $t("painter.hint_xpost") }}</p>
            </div>
          </td>
        </tr>
        <tr>
          <td><b>{{ $t("painter.edit") }}</b></td>
          <td>
            <div class="response-items">
              <button v-on:click="edit" ref="upload-button">{{ $t("painter.edit_go_back_to_editor") }}</button>
              <p class="response-hint">{{ $t("painter.hint_edit") }}</p>
            </div>
          </td>
        </tr>
        <tr>
          <td><b>{{ $t("painter.upload_confirm") }}</b></td>
          <td>
            <div class="response-items">
              <button v-on:click="uploadImage" ref="upload-button">{{ $t("painter.upload") }}</button>
              <p class="response-hint">{{ $t("painter.hint_confirm") }}</p>
            </div>
          </td>
        </tr>
      </tbody>
    </table>
  </PanelLayout>
</template>

<style scoped>
.image-container {
  display: flex;
  justify-content: center;
  width: 100%;
  max-height: 400px;
  background-size: 8px 8px;
  background-image: repeating-linear-gradient(45deg, #FFB6C1 0, #FFB6C1 0.8px, #FFFFFF 0, #FFFFFF 50%);
}

.image-container img {
  max-height: 400px;
}

.response-extra {
  width: 90%;
}

.response-extra input[type=text] {
  width: 100%;
}

/* left-hand side of form - label */
.response-extra td:has(b) {
  padding-left: 0.75em;
  display: block;
}

/* right-hand side of form - options */
.response-extra td:has(div) {
  padding-left: 0.25em;
}

.response-items {
  padding-bottom: 0.6em;
}

.response-hint {
  margin: 0.15em 0;
  font-size: 10pt;
}

.response-tools {
  padding: 10px;
  display: flex;
  flex-direction: column;
  justify-content: center;
}

.response-tools * {
  margin-bottom: 10px;
}

.tag-input {
  border: 1px solid #FFB6C1;
  display: flex;
  flex-direction: row;
  align-items: center;
  padding: 5px;
}

.tag-input * {
  margin: 0px 5px 0px 0px !important;
}

.tag-input input {
  flex: 1;
  border: none;
  outline: none;
}

.tag-input input:focus {
  border: none;
  outline: none;
}

button {
  font-size: 14pt;
}

input[type=checkbox] {
  accent-color: #FFB6C1;
}
</style>

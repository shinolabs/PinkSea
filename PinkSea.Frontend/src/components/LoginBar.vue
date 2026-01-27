<script setup lang="ts">
import { ref, useTemplateRef } from 'vue'
import i18next from 'i18next'
import { xrpc } from '@/api/atproto/client'
import { useAlert } from '@/api/modals/alert'

const handle = ref<string>('')
const password = ref<string>('')

const loginButton = useTemplateRef<HTMLButtonElement>('login-button')

const { showErrorAlert } = useAlert()

const beginOAuth = async () => {
    loginButton.value!.disabled = true
    try {
        const { data } = await xrpc.call(
            'com.shinolabs.pinksea.beginLoginFlow',
            {
                data: {
                    handle: handle.value,
                    redirectUrl: `${location.origin}/callback`,

                    password: password.value.length > 0 ? password.value : null,
                },
            },
        )

        if (data.redirect !== null && data.redirect !== undefined) {
            document.location = data.redirect
        }
    } catch (e) {
        if ('kind' in e) {
            showErrorAlert(`login.${e.kind.toLowerCase()}`)
        } else {
            showErrorAlert('login.clienterror')
            console.log(e)
        }
        loginButton.value!.disabled = false
    }
}
</script>

<template>
    <div>
        <input
            type="text"
            v-on:keydown.enter="beginOAuth"
            :placeholder="i18next.t('menu.input_placeholder')"
            v-model="handle"
        />
        <input
            type="password"
            v-on:keydown.enter="beginOAuth"
            :placeholder="i18next.t('menu.password')"
            :title="i18next.t('menu.oauth2_info')"
            v-model="password"
        />
        <br />
        <button v-on:click.prevent="beginOAuth" ref="login-button">
            {{ $t('menu.atp_login') }}
        </button>
    </div>
</template>

<style scoped></style>

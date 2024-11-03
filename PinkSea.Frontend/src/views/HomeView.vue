<script setup lang="ts">

import { ref } from 'vue'

const handle = ref<string>("");

const beginOAuth = () => {
  document.location = "/oauth/begin-flow?handle=" + handle.value;
};

</script>

<template>
  <main>
    <img src="@/assets/logo.png" alt="PinkSea logo">
    <div class="name">PinkSea</div>
    <div class="login">
      <input placeholder="@your.cool.name" v-model="handle"><button v-on:click.prevent="beginOAuth()">@ Login via ATProto</button>
    </div>
  </main>
</template>

<style scoped>
  main {
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    height: 100%;
  }
  main * {
    margin-top: 10px;
  }
  img {
    max-width: 300px;
  }
  .name {
    font-size: 100px;
    font-family: "Roboto", sans-serif;
    font-weight: 900;
    font-style: normal;
    background: linear-gradient(329deg, rgba(240,185,255,1) 0%, rgba(246,50,255,1) 100%);
    background-clip: text;
    -webkit-text-fill-color: transparent;
  }
  .login * {
    margin-right: 10px;
    padding: 10px;
  }

  .login button {
    background-color: #00a2ed;
    color: #fff;
    border: none;
    font-family: "Roboto", sans-serif;
    font-weight: 900;
    padding: 10px 20px;
    font-size: 16px;
    border-radius: 8px;
    cursor: pointer;
    transition: background-color 0.3s ease;
  }

  .login button:hover {
    background-color: #00a2ed;
  }

  .login input {
    padding: 10px;
    font-size: 16px;
    border: 1px solid #ccc;
    border-radius: 8px;
    outline: none;
    transition: border-color 0.3s ease;
  }

  .login input:focus {
    border-color: #87CEEB; /* light blue border on focus */
  }
</style>

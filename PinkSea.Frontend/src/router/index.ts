import { createRouter, createWebHistory, type RouteParamsGeneric } from 'vue-router'
import HomeView from '@/views/HomeView.vue'
import CallbackView from '@/views/CallbackView.vue'
import PainterView from '@/views/PainterView.vue'
import UserView from '@/views/UserView.vue'
import { withBreadcrumb } from '@/api/breadcrumb/breadcrumb'
import PostView from '@/views/PostView.vue'
import { xrpc } from '@/api/atproto/client'
import TagView from '@/views/TagView.vue'
import SettingsView from '@/views/SettingsView.vue'
import i18next from 'i18next'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView,
      meta: {
        resolveBreadcrumb: async () => {
          return { name: "breadcrumb.recent" };
        }
      }
    },
    {
      path: '/callback',
      name: 'callback',
      component: CallbackView
    },
    {
      path: '/paint',
      name: 'paint',
      component: PainterView,
      meta: {
        resolveBreadcrumb: async () => {
          return { name: "breadcrumb.painter" };
        }
      }
    },
    {
      path: '/:did',
      name: 'user',
      component: UserView,
      meta: {
        resolveBreadcrumb: async (route: RouteParamsGeneric) => {
          const { data } = await xrpc.get("com.shinolabs.pinksea.getHandleFromDid", { params: { did: route.did as string }});
          return { name: 'breadcrumb.user_profile', params: { handle: data.handle } };
        }
      }
    },
    {
      path: '/:did/oekaki/:rkey',
      name: 'post',
      component: PostView,
      meta: {
        resolveBreadcrumb: async (route: RouteParamsGeneric) => {
          const { data } = await xrpc.get("com.shinolabs.pinksea.getHandleFromDid", { params: { did: route.did as string }});
          return { name: "breadcrumb.user_post", params: { handle: data.handle } };
        }
      }
    },
    {
      path: '/tag/:tag',
      name: 'tag',
      component: TagView,
      meta: {
        resolveBreadcrumb: async (route: RouteParamsGeneric) => {
          return { name: "breadcrumb.tagged", params: { tag: route.tag } };
        }
      }
    },
    {
      path: '/settings',
      name: 'settings',
      component: SettingsView,
      meta: {
        resolveBreadcrumb: async () => {
          return {  name: 'breadcrumb.settings' };
        }
      }
    }
  ]
});

withBreadcrumb(router);

export default router

import { createRouter, createWebHistory, type RouteParamsGeneric } from 'vue-router'
import HomeView from '@/views/HomeView.vue'
import CallbackView from '@/views/CallbackView.vue'
import PainterView from '@/views/PainterView.vue'
import UserView from '@/views/UserView.vue'
import { withBreadcrumb } from '@/api/breadcrumb/breadcrumb'
import PostView from '@/views/PostView.vue'
import { xrpc } from '@/api/atproto/client'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView,
      meta: {
        resolveBreadcrumb: async () => {
          return "recent";
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
          return "painter";
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
          return `${data.handle}'s profile`;
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
          return `${data.handle}'s post`;
        }
      }
    }
  ]
});

withBreadcrumb(router);

export default router

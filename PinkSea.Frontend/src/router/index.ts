import { createRouter, createWebHistory, type RouteParamsGeneric } from 'vue-router'
import HomeView from '@/views/HomeView.vue'
import CallbackView from '@/views/CallbackView.vue'
import PainterView from '@/views/PainterView.vue'
import UserView from '@/views/UserView.vue'
import { withBreadcrumb } from '@/api/breadcrumb/breadcrumb'
import PostView from '@/views/PostView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView,
      meta: {
        resolveBreadcrumb: () => {
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
        resolveBreadcrumb: () => {
          return "painter";
        }
      }
    },
    {
      path: '/:did',
      name: 'user',
      component: UserView,
      meta: {
        resolveBreadcrumb: (route: RouteParamsGeneric) => {
          return `${route.did}'s profile`;
        }
      }
    },
    {
      path: '/:did/oekaki/:rkey',
      name: 'post',
      component: PostView,
      meta: {
        resolveBreadcrumb: (route: RouteParamsGeneric) => {
          return `${route.did}'s post`;
        }
      }
    }
  ]
});

withBreadcrumb(router);

export default router

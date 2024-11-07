import type { Router } from 'vue-router'
import { useBreadcrumbBarStore } from '@/api/breadcrumb/store'

window.addEventListener('popstate', _ => {
  const bar = useBreadcrumbBarStore();
  bar.pop = true;
});

export const withBreadcrumb = (router: Router) : void => {
  router.beforeEach((to, from, next) => {
    const bar = useBreadcrumbBarStore();
    if (bar.pop) {
      bar.crumbs.pop();
      bar.pop = false;
      next();
      return;
    }

    bar.crumbs.push(to.path);
    next();
  });
};

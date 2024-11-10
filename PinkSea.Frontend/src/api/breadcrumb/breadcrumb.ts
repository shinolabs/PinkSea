import type { Router } from 'vue-router'
import { useBreadcrumbBarStore } from '@/api/breadcrumb/store'
import type Crumb from '@/api/breadcrumb/crumb'

window.addEventListener('popstate', () => {
  const bar = useBreadcrumbBarStore();
  bar.pop = true;
});

export const withBreadcrumb = (router: Router) : void => {
  router.beforeEach(async (to, from, next) => {
    const bar = useBreadcrumbBarStore();
    if (bar.pop) {
      bar.crumbs.pop();
      bar.pop = false;
      next();
      return;
    }

    const maybeIndexOfExisting = bar.crumbs.findIndex((c: Crumb) => c.path === to.path);
    if (maybeIndexOfExisting > -1) {
      bar.crumbs.splice(maybeIndexOfExisting + 1);
      document.title = `${bar.crumbs[maybeIndexOfExisting].name} / PinkSea`;
      next();
      return;
    }

    if (to.meta.resolveBreadcrumb === null || to.meta.resolveBreadcrumb === undefined) {
      next();
      return;
    }

    const crumb: Crumb = {
      path: to.path,
      // @ts-expect-error This is 100% fine and I know it.
      name: (await to.meta.resolveBreadcrumb(to.params)) as string
    };

    document.title = `${crumb.name} / PinkSea`;

    bar.crumbs.push(crumb);
    next();
  });
};

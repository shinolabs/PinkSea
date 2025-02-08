import type { Router } from 'vue-router'
import { useBreadcrumbBarStore } from '@/api/breadcrumb/store'
import type Crumb from '@/api/breadcrumb/crumb'
import i18next from 'i18next'
import type CrumbI18nData from '@/api/breadcrumb/crumb-i18n-data'

window.addEventListener('popstate', () => {
  const bar = useBreadcrumbBarStore();
  bar.pop = true;
});

export const resolveCrumb = (crumb: Crumb): string => {
  // @ts-expect-error This is 100% fine and I know it.
  return i18next.t(crumb.i18n.name, crumb.i18n.params);
};

export const withBreadcrumb = (router: Router) : void => {
  router.beforeEach(async (to, from, next) => {
    const bar = useBreadcrumbBarStore();
    if (bar.pop) {
      bar.crumbs.pop();
      bar.pop = false;
      if (bar.crumbs.length > 0) {
        document.title = `${resolveCrumb(bar.crumbs[bar.crumbs.length - 1])} / PinkSea`;
        next();
        return;
      }
    }

    const maybeIndexOfExisting = bar.crumbs.findIndex((c: Crumb) => c.path === to.path);
    if (maybeIndexOfExisting > -1) {
      bar.crumbs.splice(maybeIndexOfExisting + 1);
      document.title = `${resolveCrumb(bar.crumbs[maybeIndexOfExisting])} / PinkSea`;
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
      i18n: (await to.meta.resolveBreadcrumb(to.params)) as CrumbI18nData
    };

    document.title = `${resolveCrumb(crumb)} / PinkSea`;

    bar.crumbs.push(crumb);
    next();
  });
};

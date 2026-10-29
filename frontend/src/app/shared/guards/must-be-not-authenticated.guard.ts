/*
 * Squidex Headless CMS
 *
 * @license
 * Copyright (c) Squidex UG (haftungsbeschränkt). All rights reserved.
 */

import { Location } from '@angular/common';
import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, Router } from '@angular/router';
import { map, take, tap } from 'rxjs/operators';
import { UIOptions } from '@app/framework';
import { AuthService } from '../services/auth.service';

export const mustBeNotAuthenticatedGuard = (route: ActivatedRouteSnapshot) => {
    const authService = inject<AuthService>(AuthService);
    const location = inject<Location>(Location);
    const options = inject<UIOptions>(UIOptions);
    const router = inject<Router>(Router);

    const redirect = options.value.redirectToLogin && !route.queryParams.logout;

    return authService.userChanges.pipe(
        take(1),
        tap(user => {
            const redirectPath = location.path(true);

            if (!user && redirect) {
                authService.loginRedirect(redirectPath);
            } else if (user) {
                router.navigate(['app'], { queryParams: { redirectPath } });
            }
        }),
        map(user => !user && !redirect));
};
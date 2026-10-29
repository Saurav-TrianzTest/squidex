/*
 * Squidex Headless CMS
 *
 * @license
 * Copyright (c) Squidex UG (haftungsbeschränkt). All rights reserved.
 */

import { Location } from '@angular/common';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { map, take, tap } from 'rxjs/operators';
import { UIOptions } from '@app/framework';
import { AuthService } from '../services/auth.service';

export const mustBeAuthenticatedGuard = () => {
    const authService = inject<AuthService>(AuthService);
    const location = inject<Location>(Location);
    const options = inject<UIOptions>(UIOptions);
    const router = inject<Router>(Router);

    return authService.userChanges.pipe(
        take(1),
        tap(user => {
            if (user) {
                return;
            }

            const redirectPath = location.path(true);

            if (options.value.redirectToLogin) {
                authService.loginRedirect(redirectPath);
            } else {
                router.navigate([''], { queryParams: { redirectPath } });
            }
        }),
        map(user => !!user));
};
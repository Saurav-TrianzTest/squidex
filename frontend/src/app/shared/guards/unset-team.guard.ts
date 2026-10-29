/*
 * Squidex Headless CMS
 *
 * @license
 * Copyright (c) Squidex UG (haftungsbeschränkt). All rights reserved.
 */

import { inject } from '@angular/core';
import { map } from 'rxjs/operators';
import { TeamsState } from '../state/teams.state';

export const unsetTeamGuard = () => {
    const teamsState = inject<TeamsState>(TeamsState);

    return teamsState.select(null).pipe(map(a => a === null));
};

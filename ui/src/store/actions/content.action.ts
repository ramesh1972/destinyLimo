import { createAction, props } from '@ngrx/store';

import { Content } from '../models/Content';

// fetch content
export const invokeContentFetchAPI = createAction(
  '[Content API] Invoke Content Fetch API'
);

export const contentFetchAPI_Success = createAction(
  '[Content API] Fetch API Success',
  props<{ allContent: Content[] }>()
);

export const invokeContentCreateAPI = createAction(
  '[Content API] Invoke Create Content API',
  props<{ content: Content }>()
);

export const createContent = createAction(
  '[Content API] Create Content',
  props<{ content: Content }>()
);

export const invokeUpdateContentAPI = createAction(
  '[Content API] Invoke Update Content API',
  props<{ content: Content }>()
);

export const updateContent = createAction(
  '[Content API] Update Content',
  props<{ content: Content }>()
);

export const invokeDeleteContentAPI = createAction(
  '[Content API] Invoke Delete Content API',
  props<{ Id: number }>()
);

export const deleteContent = createAction(
  '[Content API] Delete Content',
  props<{ id: number }>()
);
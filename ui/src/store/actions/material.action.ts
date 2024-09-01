import { createAction, props } from '@ngrx/store';

import { MaterialCategory } from '../models/MaterialCategory';
import { TrainingMaterial } from '../models/Material'
import { MaterialFile } from '../models/MaterialFile';
import { MaterialText } from '../models/MaterialText';
import { MaterialVideo } from '../models/MaterialVideo';
import { MaterialMCQ } from '../models/MaterialMCQ';

// fetch MaterialCategory
export const invokeMaterialCategoryFetchAPI = createAction(
  '[MaterialCategory API] Invoke MaterialCategory Fetch API'
);

export const materialCategoryFetchAPI_Success = createAction(
  '[MaterialCategory API] Fetch API Success',
  props<{ allMaterialCategory: MaterialCategory[] }>()
);

// fetch TrainingMaterial
export const invokeMaterialFetchAPI = createAction(
  '[Material API] Invoke Material Fetch API'
);

export const materialFetchAPI_Success = createAction(
  '[Material API] Fetch API Success',
  props<{ allMaterials: TrainingMaterial[] }>()
);

// fetch MaterialFile
export const invokeMaterialFileFetchAPI = createAction(
  '[MaterialFile API] Invoke MaterialFile Fetch API'
);

export const materialFileFetchAPI_Success = createAction(
  '[MaterialFile API] Fetch API Success',
  props<{ allMaterialFiles: MaterialFile[] }>()
);

// fetch MaterialText
export const invokeMaterialTextFetchAPI = createAction(
  '[MaterialText API] Invoke MaterialText Fetch API'
);

export const materialTextFetchAPI_Success = createAction(
  '[MaterialText API] Fetch API Success',
  props<{ allMaterialTexts: MaterialText[] }>()
);

// fetch MaterialVideo
export const invokeMaterialVideoFetchAPI = createAction(
  '[MaterialVideo API] Invoke MaterialVideo Fetch API'
);

export const materialVideoFetchAPI_Success = createAction(
  '[MaterialVideo API] Fetch API Success',
  props<{ allMaterialVideos: MaterialVideo[] }>()
);

// fetch MaterialMCQ
export const invokeMaterialMCQFetchAPI = createAction(
  '[MaterialMCQ API] Invoke MaterialMCQ Fetch API'
);

export const materialMCQFetchAPI_Success = createAction(
  '[MaterialMCQ API] Fetch API Success',
  props<{ allMaterialMCQs: MaterialMCQ[] }>()
);

export const invokeMaterialMCQ_CreateAPI = createAction(
  '[MaterialMCQ API] Invoke MaterialMCQ Create API',
  props<{ materialMCQ: MaterialMCQ }>()
);

export const materialMCQ_CreateAPI_Success = createAction(
  '[MaterialMCQ API] Create API Success',
  props<{ materialMCQ: MaterialMCQ }>()
);

export const invokeMaterialMCQ_UpdateAPI = createAction(
  '[MaterialMCQ API] Invoke MaterialMCQ Update API',
  props<{ materialMCQ: MaterialMCQ }>()
);

export const materialMCQ_UpdateAPI_Success = createAction(
  '[MaterialMCQ API] Update API Success',
  props<{ materialMCQ: MaterialMCQ }>()
);

export const invokeMaterialMCQ_DeleteAPI = createAction(
  '[MaterialMCQ API] Invoke MaterialMCQ Delete API',
  props<{ question_id: number }>()
);

export const materialMCQ_DeleteAPI_Success = createAction(
  '[MaterialMCQ API] Delete API Success',
  props<{ question_id: number }>()
);




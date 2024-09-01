import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { select, Store } from '@ngrx/store';
import { catchError, map, mergeMap, switchMap, withLatestFrom } from 'rxjs/operators';
import { Observable, throwError } from 'rxjs';

import { MaterialCategory } from "../models/MaterialCategory";
import { TrainingMaterial } from '../models/Material'
import { MaterialFile } from '../models/MaterialFile';
import { MaterialText } from '../models/MaterialText';
import { MaterialVideo } from '../models/MaterialVideo';
import { MaterialMCQ } from '../models/MaterialMCQ';

import { MaterialService } from '../apis/material.service';

import {
  invokeMaterialCategoryFetchAPI, materialCategoryFetchAPI_Success
  , invokeMaterialFetchAPI, materialFetchAPI_Success
  , invokeMaterialFileFetchAPI, materialFileFetchAPI_Success
  , invokeMaterialTextFetchAPI, materialTextFetchAPI_Success
  , invokeMaterialVideoFetchAPI, materialVideoFetchAPI_Success
  , invokeMaterialMCQFetchAPI, materialMCQFetchAPI_Success,
  invokeMaterialMCQ_UpdateAPI, materialMCQ_UpdateAPI_Success,
  invokeMaterialMCQ_CreateAPI, materialMCQ_CreateAPI_Success,
  invokeMaterialMCQ_DeleteAPI, materialMCQ_DeleteAPI_Success

} from '../actions/material.action';

@Injectable()
export class MaterialEffect {
  constructor(
    private actions$: Actions,
    private materialService: MaterialService,
    private store: Store
  ) { }

  loadAllMaterialCategory$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(invokeMaterialCategoryFetchAPI),
      mergeMap(() => {
        return this.materialService
          .getMaterialCategory()
          .pipe(map((data: MaterialCategory[]) => materialCategoryFetchAPI_Success({ allMaterialCategory: data as MaterialCategory[] }))
          );
      }));
  });

  loadAllMaterial$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(invokeMaterialFetchAPI),
      mergeMap(() => {
        return this.materialService
          .getMaterial()
          .pipe(map((data: TrainingMaterial[]) => materialFetchAPI_Success({ allMaterials: data as TrainingMaterial[] }))
          );
      }));
  });

  loadAllMaterialFile$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(invokeMaterialFileFetchAPI),
      mergeMap((action) => {
        return this.materialService
          .getMaterialFile(action.isPublic)
          .pipe(map((data: MaterialFile[]) => materialFileFetchAPI_Success({ allMaterialFiles: data as MaterialFile[] }))
          );
      }));
  });

  loadAllMaterialText$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(invokeMaterialTextFetchAPI),
      mergeMap((action) => {
        return this.materialService
          .getMaterialText(action.isPublic)
          .pipe(map((data: MaterialText[]) => materialTextFetchAPI_Success({ allMaterialTexts: data as MaterialText[] }))
          );
      }));
  });

  loadAllMaterialVideo$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(invokeMaterialVideoFetchAPI),
      mergeMap((action) => {
        return this.materialService
          .getMaterialVideo(action.isPublic)
          .pipe(map((data: MaterialVideo[]) => materialVideoFetchAPI_Success({ allMaterialVideos: data as MaterialVideo[] }))
          );
      }));
  });

  loadAllMaterialMCQ$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(invokeMaterialMCQFetchAPI),
      mergeMap((action) => {
        return this.materialService
          .getMaterialMCQ(action.isPublic)
          .pipe(map((data: MaterialMCQ[]) => materialMCQFetchAPI_Success({ allMaterialMCQs: data as MaterialMCQ[] }))
          );
      }));
  });

  updateMaterialMCQ$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(invokeMaterialMCQ_UpdateAPI),
      mergeMap((action: any) => {
        return this.materialService
          .updateTrainingMaterialMCQ(action.materialMCQ)
          .pipe(map(() => materialMCQ_UpdateAPI_Success({ materialMCQ: action.materialMCQ }))
          );
      }));
  });

  createMaterialMCQ$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(invokeMaterialMCQ_CreateAPI),
      mergeMap((action: any) => {
        return this.materialService
          .createTrainingMaterialMCQ(action.materialMCQ)
          .pipe(map(() => materialMCQ_CreateAPI_Success({ materialMCQ: action.materialMCQ }))
          );
      }));
  });

  deleteMaterialMCQ$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(invokeMaterialMCQ_DeleteAPI),
      mergeMap((action: any) => {
        return this.materialService
          .deleteTrainingMaterialMCQ(action.Id)
          .pipe(map(() => materialMCQ_DeleteAPI_Success({ question_id: action.Id }))
          );
      }));
  });
}

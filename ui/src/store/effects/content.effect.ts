import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { select, Store, Action } from '@ngrx/store';
import { catchError, map, mergeMap, switchMap, withLatestFrom } from 'rxjs/operators';
import { Observable, throwError } from 'rxjs';

import { Content } from '../models/Content';
import { ContentService } from '../apis/content.service';

import { invokeContentFetchAPI, contentFetchAPI_Success, invokeContentCreateAPI, invokeUpdateContentAPI, invokeDeleteContentAPI } from '../actions/content.action';

@Injectable()
export class ContentEffect {
  constructor(
    private actions$: Actions,
    private contentService: ContentService,
    private store: Store
  ) { }

  loadAllContent$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(invokeContentFetchAPI),
      mergeMap(() => {
        return this.contentService
          .getContent()
          .pipe(map((data: Content[]) => contentFetchAPI_Success({ allContent: data as Content[] }))
          );
      }));
  });


  updateContent$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(invokeUpdateContentAPI),
      switchMap(action => {
        return this.contentService
          .updateContent(action.content)
          .pipe(
            map(() => {
              console.log('content updated');
              return { type: 'UPDATE_CONTENT_SUCCESS' } as Action<string>;
            }),
            catchError((error: any) => {
              console.error('error updating content', error);
              return throwError(error);
            })
          );
      })
    );
  });

  createContent$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(invokeContentCreateAPI),
      switchMap(action => {
        return this.contentService
          .createContent(action.content)
          .pipe(
            map(() => {
              console.log('content created');
              return { type: 'CREATE_CONTENT_SUCCESS' } as Action<string>;
            }),
            catchError((error: any) => {
              console.error('error creating content', error);
              return throwError(error);
            })
          );
      })
    );
  });

  deleteContent$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(invokeDeleteContentAPI),
      switchMap(action => {
        return this.contentService
          .deleteContent(action.Id)
          .pipe(
            map(() => {
              console.log('content deleted');
              return { type: 'DELETE_CONTENT_SUCCESS' } as Action<string>;
            }),
            catchError((error: any) => {
              console.error('error deleting content', error);
              return throwError(error);
            })
          );
      })
    );
  });
}

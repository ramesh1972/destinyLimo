// // import the interface
import { createReducer, on } from '@ngrx/store';

import { Content } from '../models/Content';
import { ContentState } from '../states/content.state';
import { contentFetchAPI_Success } from '../actions/content.action';

//create a dummy initial state
export const initialState: ContentState = {
  content: []
};

export const contentReducer = createReducer(
  initialState,
  on(contentFetchAPI_Success, (state, { allContent }) => {
    return { ...state, content: allContent };
  }),
);


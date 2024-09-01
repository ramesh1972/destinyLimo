import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Store } from '@ngrx/store';
import { take } from 'rxjs/operators';
import { Actions, ofType } from '@ngrx/effects';

// CoreUI Modules
import { ButtonModule, DropdownModule } from '@coreui/angular';
import { CardModule } from '@coreui/angular';
import { FormModule, FormCheckComponent } from '@coreui/angular';
import { ButtonDirective } from '@coreui/angular';

import { AccordianParentComponent } from '@src/components/common/components/accordian-parent/accordian-parent.component';
import { invokeMaterialMCQFetchAPI, materialMCQFetchAPI_Success } from '@src/store/actions/material.action';
import { invokeMaterialMCQ_CreateAPI, invokeMaterialMCQ_UpdateAPI, invokeMaterialMCQ_DeleteAPI } from '@src/store/actions/material.action';
import { selectMaterialCategorys, selectMaterialMCQs } from '@src/store/selectors/material.selector';

@Component({
  selector: 'app-exams',
  standalone: true,
  imports: [CommonModule, FormModule, FormCheckComponent, ButtonDirective,
    FormsModule,
    ButtonModule,
    CardModule,
    FormModule,
    DropdownModule,
    AccordianParentComponent],
  templateUrl: './exams.component.html',
  styleUrl: './exams.component.css'
})
export class ExamsComponent {
  constructor(private readonly store: Store, private actions$: Actions) {
  }

  categories: any[] = [];
  mcqs: any[] = [];
  newPostString = 'Add New MCQ';
  selectedCategoryName = '';

  ngOnInit() {

    // set data
    console.log("material fetch dispatched");
    this.store.select(selectMaterialCategorys).subscribe((data: any) => {
      console.log('cats fetched', data);

      this.categories = [...data];
    });

    this.store.dispatch(invokeMaterialMCQFetchAPI())

    // Wait for the action to complete
    this.actions$.pipe(
      ofType(materialMCQFetchAPI_Success),
      take(1)
    ).subscribe(() => {
      console.log("content fetch dispatched");

      this.store.select(selectMaterialMCQs).subscribe((data: any) => {
        console.log("cats ", this.categories);
        console.log('material fetched', data);

        const mcqs = data.map((mcq: any) => {
          console.log("mat cat", mcq.material_category_id);
          const catName = this.categories.find((cat: any) => cat.id === mcq.material_category_id)?.category_name || 'Undefined';
          console.log('catName', catName);

          return {
            ...mcq,
            title: mcq.question_text || 'MCQ Question',
            category_name: catName,
            editing: false,
            adding: false
          };
        });

        this.mcqs = [...mcqs];
        console.log("mods mcqs", this.mcqs)
      });
    });
  }

  getCurrentCategoryName(mcq: any): string {
    return this.selectedCategoryName || mcq.category_name || 'Select a Category';
  }

  selectCategory(categoryName: string, index: number): void {
    console.log('selectCategory', categoryName, index);
    this.selectedCategoryName = categoryName;
    this.mcqs[index].category_name = categoryName; // Update the mcq object if needed
  }

  adjustTextareaHeight(event: Event): void {
    console.log('adjustTextareaHeight');
    const textarea = event.target as HTMLTextAreaElement;
    textarea.style.height = 'auto';
    textarea.style.height = `${textarea.scrollHeight}px`;
  }

  onAdd() {
    console.log('component onAdd');

    this.selectedCategoryName = '';

    this.mcqs.unshift({
      title: 'MCQ Title',
      description: '',
      content_type_id: 4,
      adding: true,
      editing: false,
      is_active: true,
      is_public: true,
      is_deleted: false
    });

    this.mcqs.forEach((item: any, index: number) => {
      item.editing = false;
    });
  }

  onEdit(i: number) {
    console.log('component edit', i);

    this.selectedCategoryName = '';

    this.mcqs.forEach((item: any, index: number) => {
      item.editing = false;
      item.adding = false;
    });

    this.mcqs[i].editing = true;
    this.mcqs[i].adding = false;
  }

  onDelete(i: number) {
    this.mcqs[i].is_deleted = true;
    this.store.dispatch(invokeMaterialMCQ_DeleteAPI(this.mcqs[i].question_id));

    this.selectedCategoryName = '';

    // remove from the list array
    this.mcqs.splice(i, 1);

    this.mcqs[i].editing = false;
    this.mcqs[i].adding = false;
  }

  onSave(i: number) {
    console.log('component save', i);

    this.selectedCategoryName = '';

    if (this.mcqs[i].adding) {
      this.store.dispatch(invokeMaterialMCQ_CreateAPI({
        materialMCQ: {
          question_id: -1,
          material_id: -1,
          category_name: this.mcqs[i].category_name,
          thumbnail: '',
          background_img: '',
          material_type_id: this.mcqs[i].content_type_id,
          material_category_id: this.categories.find((cat: any) => cat.category_name === this.mcqs[i].category_name)?.id || -1,
          title: this.mcqs[i].title,
          description: this.mcqs[i].description,
          question_text: this.mcqs[i].question_text,
          answer_1: this.mcqs[i].answer_1,
          answer_2: this.mcqs[i].answer_2,
          answer_3: this.mcqs[i].answer_3,
          answer_4: this.mcqs[i].answer_4,
          correct_1: this.mcqs[i].correct_1,
          correct_2: this.mcqs[i].correct_2,
          correct_3: this.mcqs[i].correct_3,
          correct_4: this.mcqs[i].correct_4,
          is_public: this.mcqs[i].is_public,
          is_active: this.mcqs[i].is_active,
          is_deleted: this.mcqs[i].is_deleted
        }
      }));
    } else if (this.mcqs[i].editing) {
      this.store.dispatch(invokeMaterialMCQ_UpdateAPI({
        materialMCQ: {
          question_id: this.mcqs[i].question_id,
          material_id: this.mcqs[i].material_id,
          category_name: this.mcqs[i].category_name,
          thumbnail: this.mcqs[i].thumbnail,
          background_img: this.mcqs[i].background_img,
          material_type_id: this.mcqs[i].content_type_id,
          material_category_id: this.categories.find((cat: any) => cat.category_name === this.mcqs[i].category_name)?.id || -1,
          title: this.mcqs[i].title,
          description: this.mcqs[i].description,
          question_text: this.mcqs[i].question_text,
          answer_1: this.mcqs[i].answer_1,
          answer_2: this.mcqs[i].answer_2,
          answer_3: this.mcqs[i].answer_3,
          answer_4: this.mcqs[i].answer_4,
          correct_1: this.mcqs[i].correct_1,
          correct_2: this.mcqs[i].correct_2,
          correct_3: this.mcqs[i].correct_3,
          correct_4: this.mcqs[i].correct_4,
          is_public: this.mcqs[i].is_public,
          is_active: this.mcqs[i].is_active,
          is_deleted: this.mcqs[i].is_deleted
        }
      }));
    }
  
    this.mcqs[i].editing = false;
    this.mcqs[i].adding = false;
  }

  onCancel(i: number) {
    console.log('component cancel', i);

    if (this.mcqs[i].adding) {
      this.mcqs.splice(i, 1);
    }

    this.mcqs[i].editing = false;
    this.mcqs[i].adding = false;
  }
}

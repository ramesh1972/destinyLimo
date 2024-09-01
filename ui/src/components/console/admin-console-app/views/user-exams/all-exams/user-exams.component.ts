import { Component } from '@angular/core';
import { Store, select } from '@ngrx/store';
import { filter, max, take } from 'rxjs/operators';
import { Actions, ofType } from '@ngrx/effects';
import { CommonModule } from '@angular/common';

import * as VTable from '@visactor/vtable';
import { ButtonDirective } from '@coreui/angular';
import { FormCheckLabelDirective } from '@coreui/angular';
import { ButtonGroupComponent } from '@coreui/angular';
import { ReactiveFormsModule, UntypedFormBuilder, UntypedFormControl, UntypedFormGroup } from '@angular/forms';
import { FormCheckComponent } from '@coreui/angular';

import { invokeMaterialMCQFetchAPI, materialMCQFetchAPI_Success } from '@src/store/actions/material.action';
import { selectMaterialCategorys, selectMaterialMCQs } from '@src/store/selectors/material.selector';

import { invokeUserProfilesFetchAPI, UserProfilesFetchAPI_Success } from '@src/store/actions/user.action';
import { invokeUserExamsFetchAPI, UserExamsFetchAPI_Success } from '@src/store/actions/exam.action';
import { selectUserExamsById } from '@src/store/selectors/exam.selector';

import { invokeUserExamByIdFetchAPI, UserExamByIdFetchAPI_Success } from '@src/store/actions/exam.action';
import { selectExamAnswers } from '@src/store/selectors/exam.selector';

import { DataGridComponentHelper } from '@src/components/common/components/grid-parent/data-grid.helper';

import { UserExamAnswersComponent } from './exam/user-exam-answers.component';

@Component({
  selector: 'app-user-exams',
  standalone: true,
  imports: [CommonModule, UserExamAnswersComponent, FormCheckComponent, ButtonDirective, FormCheckLabelDirective,
    ButtonGroupComponent, ReactiveFormsModule,],
  templateUrl: './user-exams.component.html',
  styleUrl: './user-exams.component.css'
})
export class UserExamsComponent {
  constructor(private readonly store: Store, private actions$: Actions) {
    // setup the data grid helper
    this.dataGridHelper = new DataGridComponentHelper(this, this.store);
  }

  formRadio1 = new UntypedFormGroup({
    radio1: new UntypedFormControl('All')
  });

  setRadioValue(value: string): void {
    this.formRadio1.setValue({ radio1: value });

    if (value === 'All') {
      this.dataGridHelper?.table?.updateFilterRules([]);
    }
    else if (value === 'Pass') {
      this.dataGridHelper?.table?.updateFilterRules([{
        filterFunc: (record: Record<string, any>) => {
          
          var percentage = record['num_questions'] !== 0 ? (record['num_correct'] / record['num_questions'] * 100) : 0;
          percentage = Math.round(percentage);
          const pass = percentage >= 50 ? "Pass" : "Fail";
          return pass === "Pass";
        }
      }]);
    }
    else if (value === 'Fail') {
      this.dataGridHelper?.table?.updateFilterRules([{
        filterFunc: (record: Record<string, any>) => {
          
          var percentage = record['num_questions'] !== 0 ? (record['num_correct'] / record['num_questions'] * 100) : 0;
          percentage = Math.round(percentage);
          const pass = percentage >= 50 ? "Pass" : "Fail";
          return pass === "Fail";
        }
      }]);
    }
  }

  dataGridHelper?: DataGridComponentHelper;

  saveButtonEnabled = false;
  resetButtonEnabled = false;

  exams: any[] = [];
  users: any[] = [];
  categories: any[] = [];
  mcqs: any[] = [];
  examAnswers: any[] = [];
  currentExam: any = {};
  showingResults = false;

  ngOnInit() {
    console.log('file component initialized');

    this.dataGridHelper?.setTableInfo('user_exams', "exam_id", false);

    // set columns
    this.dataGridHelper?.setColumns(this.getColDefs());

    // fetch data
    // set data
    console.log("material fetch dispatched");
    this.store.select(selectMaterialCategorys).subscribe((data: any) => {
      console.log('cats fetched', data);

      this.categories = [...data];
    });

    this.store.dispatch(invokeMaterialMCQFetchAPI({isPublic: false}));

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
          const catName = this.categories.find((cat: any) => cat.id === mcq.material_category_id)?.category_name || 'Undefined';

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

    // users
    this.store.dispatch(invokeUserProfilesFetchAPI());

    this.actions$.pipe(
      ofType(UserProfilesFetchAPI_Success),
      take(1)
    ).subscribe((data: any) => {
      console.log("users fetch dispatched", data);

      this.users = [...data.allUserProfiles];
    });

    // set data
    this.store.dispatch(invokeUserExamsFetchAPI({onlyExamsNotStarted: false}));
    this.actions$.pipe(
      ofType(UserExamsFetchAPI_Success),
      take(1)
    ).subscribe((data: any) => {
      console.log("exams fetch dispatched", data);

      const exams = data.allExams.map((exam: any) => {

        var user = this.users.find((user: any) => user.userId === exam.userId);
        if (user !== undefined && user !== null)
          console.log("found user", user);
        return {
          ...exam,
          userFullName: user ? user.firstName + " " + user.lastName : "Not Found",
          avatar: user ? user.avatar : 'images/avatars/blank-avatar.webp',
        };
      });

      console.log("modified files :", exams)
      this.dataGridHelper!.setData(exams);

      // ----> draw the table
      this.drawVTable();
    });
  }

  // columns definition
  // columns definition
  getColDefs() {
    return [
      {
        field: 'examId',
        title: 'View QnAs',
        width: 100,
        columnResizeMode: 'none',
        disableColumnResize: true,
        maxWidth: 100,
        style: {
          textAlign: 'center',
          bgColor: 'rgba(184, 132, 132, 0.1)',
          textBaseline: "middle",
        },
        fieldFormat: ((record: any) => {
          return "";
        }),
        icon: [
          {
            name: 'view',
            type: 'svg',
            positionType: VTable.TYPES.IconPosition.left,
            marginLeft: 26,
            width: 20,
            height: 20,
            svg: '/images/buttons/view.png',
            tooltip: {
              title: 'view answers',
              placement: VTable.TYPES.Placement.right
            }
          },
        ]
      },
      {
        title: "Learner", field: "userId",
        cellType: 'text',
        width: 90,
        sort: true,
        textStick: true,
        style: {
          bgColor: 'rgba(184, 132, 132, 0.3)',
          textBaseline: "middle"
        },
        customLayout: (args: any) => {
          const { table, row, col, rect } = args;
          const record = table.getCellOriginRecord(col, row);
          const { height, width } = rect ?? table.getCellRect(col, row);
          const percentCalc = VTable.CustomLayout.percentCalc;

          const container = new VTable.CustomLayout.Group({
            height,
            width,
            display: 'flex',
            flexDirection: 'row',
            flexWrap: 'nowrap',
            alignItems: 'center',
            boundsPadding: [10, 10, 10, 10],
          });

          const containerLeft: any = new VTable.CustomLayout.Group({
            height: 50,
            width: 150,

            display: 'flex',
            flexDirection: 'row',
            background: 'transparent',
            boundsPadding: [10, 10],
            cornerRadius: 6,
            alignItems: 'flex-start',
          });

          container.add(containerLeft);

          const avatar = new VTable.CustomLayout.Image({
            src: record.avatar,
            width: 40,
            height: 40,
            boundsPadding: [4, 4],
            cornerRadius: 20,
          });

          containerLeft.add(avatar);

          const userName = new VTable.CustomLayout.Text({
            text: record.userFullName,
            fontSize: 13,
            fontFamily: 'sans-serif',
            fill: 'black',
            lineHeight: 20,
            boundsPadding: [4, 10],
            marginTop: 15
          });

          containerLeft.add(userName);

          return {
            rootContainer: container,
            renderDefault: false
          };
        }
      },
      {
        title: "Dates", field: "dateCreated",
        cellType: 'text',
        width: 260,
        maxWidth: 260,
        columnResizeMode: 'none',
        disableColumnResize: true,
        style: {
          bgColor: 'white',
          textBaseline: "middle",
          selfAlign: 'center',
        },
        customLayout: (args: any) => {
          const { table, row, col, rect } = args;
          const record = table.getCellOriginRecord(col, row);
          const { height, width } = rect ?? table.getCellRect(col, row);
          const percentCalc = VTable.CustomLayout.percentCalc;

          const container = new VTable.CustomLayout.Group({
            height,
            width,
            display: 'flex',
            flexDirection: 'column',
            flexWrap: 'nowrap',
          });

          const containerLeft: any = new VTable.CustomLayout.Group({
            height: percentCalc(100, -60).percent,
            width: percentCalc(100, -60).percent,
            display: 'flex',
            flexDirection: 'column',
            alignItems: 'flex-start',
            background: 'white',
            boundsPadding: [10, 10, 10, 10],
            justifyContent: 'space-between',
            cornerRadius: 6,
          });

          container.add(containerLeft);

          const dateCreated = new VTable.CustomLayout.Text({
            text: "Created on      " + new Date(record.dateCreated).toLocaleString(),
            fontSize: 13,
            fontFamily: 'sans-serif',
            fill: 'blue',
            lineHeight: 20,
            boundsPadding: [4, 4]

          });

          containerLeft.add(dateCreated);

          const dateStarted = new VTable.CustomLayout.Text({
            text: "Started on       " + new Date(record.dateStarted).toLocaleString(),
            fontSize: 13,
            fontFamily: 'sans-serif',
            fill: 'blue',
            lineHeight: 20,
            boundsPadding: [4, 4]
          });

          containerLeft.add(dateStarted);

          const dateCompleted = new VTable.CustomLayout.Text({
            text: "Completed on  " + new Date(record.dateCompleted).toLocaleString(),
            fontSize: 13,
            fontFamily: 'sans-serif',
            fill: 'blue',
            lineHeight: 20,
            boundsPadding: [4, 4]
          });

          containerLeft.add(dateCompleted);

          return {
            rootContainer: container,
            renderDefault: false
          };
        }
      },
      {
        title: "Results", field: "score",
        cellType: 'text',
        width: 280,
        maxWidth: 280,
        columnResizeMode: 'none',
        disableColumnResize: true,
        style: {
          bgColor: 'lightblue',
          textBaseline: "middle",
        },
        customLayout: (args: any) => {
          const { table, row, col, rect } = args;
          const record = table.getCellOriginRecord(col, row);
          const { height, width } = rect ?? table.getCellRect(col, row);
          const percentCalc = VTable.CustomLayout.percentCalc;

          const container = new VTable.CustomLayout.Group({
            height,
            width,
            display: 'flex',
            flexDirection: 'row',
            flexWrap: 'nowrap'
          });

          const containerLeft: any = new VTable.CustomLayout.Group({
            height: percentCalc(100, -60).percent,
            width: percentCalc(100, -60).percent,
            display: 'flex',
            flexDirection: 'column',
            alignItems: 'flex-start',
            background: 'lightblue',
            boundsPadding: [10, 10, 10, 10],
            justifyContent: 'space-between',
            cornerRadius: 6,
          });

          container.add(containerLeft);

          containerLeft.add(new VTable.CustomLayout.Text({
            text: "Correct Answers: " + record.num_correct + "/" + record.num_questions,
            fontSize: 13,
            fontFamily: 'sans-serif',
            fill: 'black',
            background: 'lightgreen',
            lineHeight: 20,
            backgroundCornerRadius: 4,
            boundsPadding: [2, 4],
            shadowColor: 'rgba(0,0,0,.6)',
            shadowBlur: 1.5,
            shadowOffsetX: .7,
            shadowOffsetY: .7
          }));

          containerLeft.add(new VTable.CustomLayout.Text({
            text: "Wrong Answers: " + (record.num_questions - record.num_correct) + "/" + record.num_questions,
            fontSize: 13,
            fontFamily: 'sans-serif',
            fill: 'black',
            background: 'red',
            lineHeight: 20,
            backgroundCornerRadius: 4,
            boundsPadding: [2, 4],
            shadowColor: 'rgba(0,0,0,.6)',
            shadowBlur: 1.5,
            shadowOffsetX: .7,
            shadowOffsetY: .7

          }));

          containerLeft.add(new VTable.CustomLayout.Text({
            text: "Attempted: " + record.num_attempted + "/" + record.num_questions,
            fontSize: 13,
            fontFamily: 'sans-serif',
            fill: 'black',
            background: 'yellow',
            lineHeight: 20,
            backgroundCornerRadius: 4,
            boundsPadding: [2, 4],
            shadowColor: 'rgba(0,0,0,.6)',
            shadowBlur: 1.5,
            shadowOffsetX: .7,
            shadowOffsetY: .7

          }));

          const containerRight: any = new VTable.CustomLayout.Group({
            height: percentCalc(100, -60).percent,
            width: percentCalc(100, -60).percent,
            display: 'flex',
            flexDirection: 'column',
            alignItems: 'flex-start',
            background: 'lightgoldenrodyellow',
            boundsPadding: [10, 10, 10, 10],
            justifyContent: 'space-around',
            cornerRadius: 6,
            shadowColor: 'rgba(0,0,0,.6)',
            shadowBlur: 1.5,
            shadowOffsetX: .7,
            shadowOffsetY: .7
          });

          var percentage = record.num_questions !== 0 ? (record.num_correct / record.num_questions * 100) : 0;
          percentage = Math.round(percentage);

          containerRight.add(new VTable.CustomLayout.Text({
            text: "Score " + percentage + "%",
            fontSize: 13,
            fontFamily: 'sans-serif',
            fill: 'black',
            background: 'transparent',
            lineHeight: 20,
            backgroundCornerRadius: 4,
            alignSelf: 'center',
          }));

          containerRight.add(new VTable.CustomLayout.Text({
            text: percentage >= 50 ? "Pass" : "Fail",
            fontSize: 16,
            fontFamily: 'sans-serif',
            fill: 'white',
            background: percentage >= 50 ? 'lightgreen' : 'red',
            lineHeight: 32,
            backgroundCornerRadius: 4,
            boundsPadding: [2, 20, 2, 20],
            alignSelf: 'center',
          }));

          container.add(containerRight);

          return {
            rootContainer: container,
            renderDefault: false
          };
        }
      },
      {
        title: "Certificate", field: "certificateUrl",
        cellType: 'link',
        width: 200,
        columnResizeMode: 'none',
        disableColumnResize: true,
        style: {
          bgColor: 'lightgoldenrodyellow',
          textBaseline: "middle",
        }
      }
    ];
  }

  getTableOptions() {

    const container = document.getElementById('tableContainer');

    // add delete column to the end
    const option = {
      container: container!,
      columns: this.dataGridHelper?.columns,

      widthMode: 'adaptive',
      heightMode: 'autoHeight',
      autoFillWidth: true,
      autoFillHeight: false,

      animationAppear: {
        duration: 100,
        delay: 50,
        type: 'one-by-one', // all
        direction: 'row' // colunm
      },

      theme: VTable.themes.ARCO
    };

    option.theme = VTable.themes.ARCO.extends({
      underlayBackgroundColor: 'transparent',
      defaultStyle: {
        autoWrapText: true,
        color: 'black',
        textBaseline: "top",
      },
      headerStyle: {
        bgColor: '#a881e1',
        borderColor: '#a881e1',
        borderLineWidth: .3,
        color: 'white',
        lineHeight: 40,
        textBaseline: "middle",
        autoWrapText: false
      },
      bodyStyle: {
        bgColor: 'white',
        borderColor: 'black',
        borderLineWidth: .3,
        color: 'black',
        textBaseline: "middle",
        autoWrapText: true
      },
      checkboxStyle: { defaultFill: 'grey' },
      frameStyle: { cornerRadius: 6, borderColor: 'black', borderLineWidth: .5, shadowColor: 'rgba(0,0,0,.6)', shadowBlur: 3, shadowOffsetX: 1.5, shadowOffsetY: 1.5 },
    });

    return option;
  }

  // styling and drawing the table
  drawVTable() {

    const option = this.getTableOptions();

    // create table
    this.dataGridHelper?.createTable(option);

    this.dataGridHelper?.table?.on('click_cell', (args: any) => {
      console.log('cell clicked', args);
      this.showingResults = true;

      if (args.col === 0) {
        this.showUserExamAnswers(args.dataValue);
      }
    });
  }

  // user exam answers
  showUserExamAnswers(examId: number) {
    console.log("examId", examId);

    this.examAnswers = [];

    this.currentExam = this.dataGridHelper?.data.find((exam: any) => exam.examId === examId);
    console.log("currentExam", this.currentExam);

    this.store.dispatch(invokeUserExamByIdFetchAPI({ examId: examId }));

    this.actions$.pipe(
      ofType(UserExamByIdFetchAPI_Success),
      take(1)
    ).subscribe((answers: any) => {
      console.log("exam answers fetched");

      const exAnswers = answers.examAnswers.map((exam: any) => {
        console.log("exam", exam);
        return {
          ...exam,
          question_text: this.mcqs.find((mcq: any) => mcq.question_id === exam.mcq_id)?.question_text || 'Undefined',
          answer_1: this.mcqs.find((mcq: any) => mcq.question_id === exam.mcq_id)?.answer_1 || 'Undefined',
          answer_2: this.mcqs.find((mcq: any) => mcq.question_id === exam.mcq_id)?.answer_2 || 'Undefined',
          answer_3: this.mcqs.find((mcq: any) => mcq.question_id === exam.mcq_id)?.answer_3 || 'Undefined',
          answer_4: this.mcqs.find((mcq: any) => mcq.question_id === exam.mcq_id)?.answer_4 || 'Undefined',
          correct_1: this.mcqs.find((mcq: any) => mcq.question_id === exam.mcq_id)?.correct_1 || false,
          correct_2: this.mcqs.find((mcq: any) => mcq.question_id === exam.mcq_id)?.correct_2 || false,
          correct_3: this.mcqs.find((mcq: any) => mcq.question_id === exam.mcq_id)?.correct_3 || false,
          correct_4: this.mcqs.find((mcq: any) => mcq.question_id === exam.mcq_id)?.correct_4 || false,

          category_name: this.mcqs.find((mcq: any) => mcq.question_id === exam.mcq_id)?.category_name || 'Undefined',
          title: this.mcqs.find((mcq: any) => mcq.question_id === exam.mcq_id)?.title || 'Undefined',
          description: this.mcqs.find((mcq: any) => mcq.question_id === exam.mcq_id)?.description || 'Undefined',
        };
      });

      const anss = exAnswers.map((exam: any) => {
        return {
          ...exam,
          is_correctly_answered: exam.choice_1_answer === exam.correct_1 && exam.choice_2_answer === exam.correct_2 && exam.choice_3_answer === exam.correct_3 && exam.choice_4_answer === exam.correct_4
        };
      });

      this.examAnswers = anss.filter((ans: any) => ans.question_text !== 'Undefined');
      console.log("exam answers modified", this.examAnswers);
    });
  };
}
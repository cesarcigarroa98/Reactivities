import { observer } from 'mobx-react-lite';
import React, { useEffect, useState } from 'react';
import { useHistory, useParams } from 'react-router';
import {Segment, Button, Header} from 'semantic-ui-react';
import LoadingComponent from '../../../app/layout/LoadingComponent';
import { ActivityFormValues } from '../../../app/models/activity';
import { useStore } from '../../../app/stores/store';
import { Link } from 'react-router-dom';
import { Formik, Form} from 'formik';
import * as Yup from 'yup';
import {v4 as uuid} from 'uuid';
import MyTextInput from '../../../app/common/form/MyTextInput';
import MyTextArea from '../../../app/common/form/MyTextArea';
import MySelectInput from '../../../app/common/form/MySelectInput';
import { categoryOptions } from '../../../app/common/options/categoryOptions';
import MyDateInput from '../../../app/common/form/MyDateInput';

export default observer(function ActivityForm () {
    /*React hook that has redirection method*/
    const history = useHistory();
    const {activityStore} = useStore();
    const {createActivity, updateActivity, loadActivity, loadingInitial} = activityStore;
    /*Receive parameter from URL */
    const {id} = useParams<{id: string}>();

    const [activity, setActivity] = useState<ActivityFormValues>(new ActivityFormValues());

    const validationSchema = Yup.object({
        title: Yup.string().required('The activity title is required'),
        description: Yup.string().required('The activity description is required'),
        category: Yup.string().required(),
        date: Yup.string().required('Date is required').nullable(),
        venue: Yup.string().required(),
        city: Yup.string().required()
    })

    useEffect(() => {
        if (id) loadActivity(id).then(activity => setActivity(new ActivityFormValues(activity)));
    }, [id, loadActivity])

    function handleFormSubmit(activity: ActivityFormValues){
        if(!activity.id) {
            let newActivity = {
                ...activity,
                /*We create new activity id on this component in order to render activity details as soon as activity is created*/
                id: uuid()
            };
            /*Redirect to activity details just after creating activity */
            createActivity(newActivity).then(() => history.push(`/activities/${newActivity.id}`))
        } else {
            /*Redirect to activity details just after updating activity */
            updateActivity(activity.id, activity).then(() => history.push(`/activities/${activity.id}`))
        }
    }

    if (loadingInitial) return <LoadingComponent content='Loading activity...'></LoadingComponent>

    return (
        <Segment clearing>
            <Header content='Activity Details' sub color='teal'/>
            <Formik 
                validationSchema = {validationSchema}
                enableReinitialize 
                initialValues={activity} 
                onSubmit={values => handleFormSubmit(values)}>
                {({handleSubmit, isValid, isSubmitting, dirty}) => (
                    <Form className='ui form' onSubmit={handleSubmit} autoComplete='off'>
                        <MyTextInput name='title' placeholder='Title'/>
                        <MyTextArea rows={3} placeholder='Description' name='description'/>
                        <MySelectInput options={categoryOptions} placeholder='Category' name='category'/>
                        <MyDateInput 
                            placeholderText='Date' 
                            name='date'
                            showTimeSelect
                            timeCaption='time'
                            dateFormat='MMMM d, yyyy h:mm aa'
                        />
                        <Header content='Location Details' sub color='teal'/>
                        <MyTextInput placeholder='City' name='city'/>
                        <MyTextInput placeholder='Venue' name='venue'/>
                        <Button 
                            disabled={isSubmitting || !dirty || !isValid}
                            loading={isSubmitting} 
                            floated='right' 
                            positive type='submit' 
                            content='Submit'>
                        </Button>
                        <Button as={Link} to='/activities' floated='right' type='button' content='Cancel'></Button>
                    </Form>
                )}
            </Formik>
        </Segment>
    );
})
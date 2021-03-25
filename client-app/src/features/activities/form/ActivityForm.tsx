import { observer } from 'mobx-react-lite';
import React, { ChangeEvent, useEffect, useState } from 'react';
import { useHistory, useParams } from 'react-router';
import {Segment, Form, Button} from 'semantic-ui-react';
import LoadingComponent from '../../../app/layout/LoadingComponent';
import { Activity } from '../../../app/models/activity';
import { useStore } from '../../../app/stores/store';
import {v4 as uuid} from 'uuid';
import { Link } from 'react-router-dom';

export default observer(function ActivityForm () {
    /*React hook that has redirection method*/
    const history = useHistory();
    const {activityStore} = useStore();
    const {createActivity, updateActivity, loading, loadActivity, loadingInitial} = activityStore;
    /*Receive parameter from URL */
    const {id} = useParams<{id: string}>();

    const [activity, setActivity] = useState<Activity>({
        id: '',
        title: '',
        category: '',
        description: '',
        date: '',
        city: '',
        venue: ''
    });

    useEffect(() => {
        if (id) loadActivity(id).then(activity => setActivity(activity!));
    }, [id, loadActivity])

    function handleSubmit(){
        if(activity.id.length === 0) {
            let newActivity = {
                ...activity,
                /*We create new activity id on this component in order to render activity details as soon as activity is created*/
                id: uuid()
            };
            /*Redirect to activity details just after creating activity */
            createActivity(newActivity).then(() => history.push(`/activities/${newActivity.id}`))
        } else {
            /*Redirect to activity details just after updating activity */
            updateActivity(activity).then(() => history.push(`/activities/${activity.id}`))
        }
    }

    /*This function assigns the input's values to the object's properties*/
    function handleInputChange(event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>){
        /*name=name of the input. value=current input's value*/
        const {name, value} = event.target;
        /*"...activity" destructures the object's properties so they can be accessed one by one*/
        setActivity({...activity, [name]: value});
    }

    if (loadingInitial) return <LoadingComponent content='Loading activity...'></LoadingComponent>

    return (
        <Segment clearing>
            <Form onSubmit={handleSubmit} autoComplete='off'>
                <Form.Input placeholder='Title' value={activity.title} name='title' onChange={handleInputChange}></Form.Input>
                <Form.TextArea placeholder='Description' value={activity.description} name='description' onChange={handleInputChange}></Form.TextArea>
                <Form.Input placeholder='Category' value={activity.category} name='category' onChange={handleInputChange}></Form.Input>
                <Form.Input type='date' placeholder='Date' value={activity.date} name='date' onChange={handleInputChange}></Form.Input>
                <Form.Input placeholder='City' value={activity.city} name='city' onChange={handleInputChange}></Form.Input>
                <Form.Input placeholder='Venue' value={activity.venue} name='venue' onChange={handleInputChange}></Form.Input>
                <Button loading={loading} floated='right' positive type='submit' content='Submit'></Button>
                <Button as={Link} to='/activities' floated='right' type='button' content='Cancel'></Button>
            </Form>
        </Segment>
    );
})
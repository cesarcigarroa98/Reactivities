import React, { Fragment, useEffect} from 'react';
import { Container} from 'semantic-ui-react';
import NavBar from './NavBar';
import ActivityDashboard from '../../features/activities/dashboard/ActivityDashboard';
import LoadingComponent from './LoadingComponent';
import { useStore } from '../stores/store';
import { observer } from 'mobx-react-lite';

function App() {
  {/*Custom hook that was created on store file*/}
  const {activityStore} = useStore();

  useEffect(() => {
    activityStore.loadActivities();     
  }, [activityStore])

  {/*Loading screen*/}
  if(activityStore.loadingInitial) return <LoadingComponent content='Loading app'></LoadingComponent>

  return (
    <Fragment>
      <NavBar></NavBar>
      <Container style={{marginTop: '7em'}}>
        <ActivityDashboard/>
      </Container>
    </Fragment>
  );
}

export default observer(App);


import { TourForCreation } from './tourForCreation.model';
import { ShowforCreation } from '../shows/shared/showForCreation';


export class TourWithShowsForCreation extends TourForCreation {
  shows: ShowforCreation[];
}

